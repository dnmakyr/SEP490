﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.DataContext;
using Microsoft.EntityFrameworkCore.Storage;
using AutoMapper;
using verbum_service_infrastructure.PagedList;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Lombok.NET;


namespace verbum_service_infrastructure.Impl.Service

{
    [RequiredArgsConstructor]
    public partial class UserServiceImpl : UserService
    {
        private readonly verbumContext context;
        private readonly IMapper mapper;
        private readonly TokenService tokenService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly MailService mailService;
        public async Task SignUp(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            Console.WriteLine("saved user to db, sending email...");
            _ = Task.Run(() => SendConfirmationEmail(user.Email));
        }
        public async Task<string> SendConfirmationEmail(string email)
        {
            var emailToken = tokenService.GenerateEmailToken(email);
            //send mail
            string body = await MailUtils.BuildVerificationEmail(SystemConfig.FE_DOMAIN + "/confirm-email" + "?access_token=" + emailToken,"mail");
            return await mailService.SendEmailAsync(email, MailConstant.CONFIRM_EMAIL_HEADER, body);
        }
        //private async Task<string> BuildVerificationEmail(string link)
        //{
        //    string path = Path.Combine("wwwroot", "assets", "HTMLTemplate", "mail.html");
        //    Console.WriteLine(path);
        //    string body = await File.ReadAllTextAsync(path);
        //    body = body.Replace("{link}", link);
        //    return body;
        //}
        public async Task<Tokens> Login(UserLogin loginCredentials)
        {
            List<string> alerts = new List<string>();

            string hashPassword = UserUtils.HashPassword(loginCredentials.Password);
            User user = await context.Users.FirstOrDefaultAsync(x => x.Password == hashPassword && x.Email == loginCredentials.Email);
            if (ObjectUtils.IsEmpty(user))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "user"));
            }
            if (ObjectUtils.IsNotEmpty(user) && user.Status != UserStatus.ACTIVE.ToString())
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "user"));
            }
            if(ObjectUtils.IsNotEmpty(user) && ObjectUtils.IsEmpty(user.EmailVerified))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.EMAIL_NOT_VERIFIED));
            }
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
            Tokens newTokens = tokenService.GenerateTokens(user);

            //transaction to roll back if update failed
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    await tokenService.UpdateRefreshToken(user.TokenId ?? 0, newTokens.RefreshToken);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return newTokens;
        }
        public async Task<Tokens> RefreshAccessToken(Tokens tokens)
        {
            List<string> alerts = new List<string>();

            var principal = tokenService.GetPrincipalFromExpiredToken(tokens.AccessToken);
            if (ObjectUtils.IsEmpty(principal))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "principal"));
            }
            string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
            User? user = context.Users.Include(x => x.Token).FirstOrDefault(context => context.Email == email);
            if (ObjectUtils.IsEmpty(user))
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "user"));
            }
            if (user.Token.ExpireAt < DateTime.Now || tokens.RefreshToken != user.Token.TokenContent)
            {
                alerts.Add(AlertMessage.Alert(ValidationAlertCode.INVALID, "refresh token"));
            }
            if (ObjectUtils.IsNotEmpty(alerts))
            {
                throw new BusinessException(alerts);
            }
            Tokens newTokens = tokenService.GenerateTokens(user);
            await tokenService.UpdateRefreshToken(user.TokenId ?? 0, newTokens.RefreshToken);
            return newTokens;
        }

        public async Task<Tokens> ConfirmEmail(string email)
        {
            User user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if(user == null)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.NOT_FOUND, "email"));
            }
            user.EmailVerified = DateTime.Now;
            user.Status = UserStatus.ACTIVE.ToString();
            Tokens tokens = tokenService.GenerateTokens(user);
            user.TokenId = await tokenService.AddRefreshToken(tokens.RefreshToken);
            await context.SaveChangesAsync();
            return tokens;
        }

        public async Task<Tokens> LoginGoogleCallback()
        {
            var authenticateResult = await httpContextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                throw new BusinessException(AlertMessage.Alert(ValidationAlertCode.LOGIN_FAIL));
            }

            var claims = authenticateResult.Principal.Claims;
            string email = claims.FirstOrDefault(c => c.Type.EndsWith("emailaddress"))?.Value;
            string name = claims.FirstOrDefault(c => c.Type.EndsWith("name"))?.Value;

            if (ObjectUtils.IsEmpty(email) && ObjectUtils.IsEmpty(name)) return null;

            User oldUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (oldUser != null)
            {
                return await LoginGoogleOldUser(oldUser);
            }

            return await LoginGoogleNewUser(email,name);
        }

        public async Task<Tokens> LoginGoogleNewUser(string email, string name)
        {
            User user = new User();
            user.Id = Guid.NewGuid();
            user.Email = email;
            user.Name = name;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.Status = UserStatus.ACTIVE.ToString();
            user.RoleCode = "CLIENT";

            context.Users.Add(user);
            await context.SaveChangesAsync();
            Tokens newTokens = tokenService.GenerateTokens(user);
            user.TokenId = await tokenService.AddRefreshToken(newTokens.RefreshToken);
            await context.SaveChangesAsync();
            return newTokens;
        }

        public async Task<Tokens> LoginGoogleOldUser(User oldUser)
        {
            Tokens newTokens = tokenService.GenerateTokens(oldUser);
            await tokenService.UpdateRefreshToken(oldUser.TokenId ?? 0, newTokens.RefreshToken);
            return newTokens;
        }

        public async Task UpdateUser(UserUpdate request)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    User user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
                    user.Name = request.Data.Name;
                    user.Email = request.Data.Email;
                    user.UpdatedAt = DateTime.Now;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<List<UserInfo>> GetAssignList()
        {
            return mapper.Map<List<UserInfo>>(await context.Users.Where(x => x.RoleCode.Equals(UserRole.LINGUIST)).ToListAsync());
        }
    }
}
