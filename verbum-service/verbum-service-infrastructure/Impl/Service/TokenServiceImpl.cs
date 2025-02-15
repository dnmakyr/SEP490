﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Models;
using verbum_service_infrastructure.DataContext;

namespace verbum_service_infrastructure.Impl.Service
{
    public class TokenServiceImpl : TokenService
    {
        private verbumContext context;
        public TokenServiceImpl(verbumContext context)
        {
            this.context = context;
        }
        public async Task<int> AddRefreshToken(string token)
        {
            RefreshToken addedToken = new RefreshToken
            {
                IssuedAt = DateTime.Now,
                ExpireAt = DateTime.Now.AddDays(SystemConfig.REFRESH_TOKEN_LIFE),
                TokenContent = token
            };
            context.RefreshTokens.Add(addedToken);
            await context.SaveChangesAsync();
            return addedToken.TokenId;
        }
        public async Task UpdateRefreshToken(int tokenId, string newToken)
        {
            int updatedRecords = await context.RefreshTokens.Where(x => x.TokenId == tokenId)
                .ExecuteUpdateAsync(u =>
                u.SetProperty(x => x.ExpireAt, DateTime.Now.AddMonths(SystemConfig.REFRESH_TOKEN_LIFE))
                .SetProperty(x => x.TokenContent, newToken)
                );
            if(updatedRecords < 1)
            {
                throw new BusinessException(ValidationAlertCode.UPDATE_RECORD_FAIL);
            }
        }
        public Tokens GenerateTokens(User userInfo)
        {
            var _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimEnum.USER_ID, userInfo.Id.ToString()),
                new Claim(ClaimEnum.EMAIL, userInfo.Email),
                new Claim(ClaimEnum.NAME, userInfo.Name),
                new Claim(ClaimEnum.STATUS, userInfo.Status),
                new Claim(ClaimEnum.ROLE, userInfo.RoleCode.ToString())
            };

            var token = new JwtSecurityToken(
              claims: claims,
              expires: DateTime.Now.AddHours(SystemConfig.ACCESS_TOKEN_LIFE),
              //expires: DateTime.Now.AddMinutes(SystemConfig.ACCESS_TOKEN_LIFE),
              audience: _config["Jwt:Audience"],
              issuer: _config["Jwt:Issuer"],
              signingCredentials: credentials);

            return new Tokens
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = GenerateRefreshToken()
            };
        }
        public string GenerateEmailToken(string email)
        {
            var _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimEnum.EMAIL, email)
            };

            var token = new JwtSecurityToken(
              claims: claims,
              expires: DateTime.Now.AddYears(MailConstant.MailExpirationTime),
              audience: _config["Jwt:Audience"],
              issuer: _config["Jwt:Issuer"],
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var Key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, //here we are saying that we don't care about the token's expiration date
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public void SetTokensInsideCookie(Tokens tokens, HttpContext context)
        {
            context.Response.Cookies.Append("access_token", tokens.AccessToken, 
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(SystemConfig.ACCESS_TOKEN_LIFE),
                    IsEssential = true, //ensure cookie always sent even when user disable cookie
                    //Secure = true,
                    SameSite = SameSiteMode.Lax
                });

            context.Response.Cookies.Append("refresh_token", tokens.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(SystemConfig.REFRESH_TOKEN_LIFE),
                    HttpOnly = true,
                    IsEssential = true,
                    //Secure = true,
                    SameSite = SameSiteMode.Lax
                });
        }
    }
}

