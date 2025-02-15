using Lombok.NET;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cmp;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using verbum_service.Filter;
using verbum_service_application.Service;
using verbum_service_domain.Common;
using verbum_service_domain.Common.ErrorModel;
using verbum_service_domain.DTO.Request;
using verbum_service_domain.DTO.Response;
using verbum_service_domain.Utils;
using verbum_service_infrastructure.Impl.Workflow;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace verbum_service.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [RequiredArgsConstructor]
    public partial class AuthenticationController : ControllerBase
    {
        private readonly CurrentUser currentUser;
        private readonly UserService userService;
        private readonly TokenService tokenService;
        private readonly CreateUserWorkflow createUserWorkflow;
        // GET: api/<AuthenticationController>
        [HttpGet]
        [Roles(UserRole.CLIENT)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        [Authorize]
        //[PermissionAuth(Entity.USER, ActionEnum.MANAGE)]
        public IEnumerable<string> Get(int id)
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<AuthenticationController>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            Tokens tokens = await userService.Login(userLogin);
            tokenService.SetTokensInsideCookie(tokens, HttpContext);
            return Ok(tokens);
        }

        [HttpGet("google-login")]
        [ProducesResponseType(typeof(Challenge), 200)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public IActionResult LoginGoogle()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/api/auth/google-callback" }, 
                GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GoogleCallback()
        {
            Tokens tokens = await userService.LoginGoogleCallback();
            tokenService.SetTokensInsideCookie(tokens, HttpContext);
            if(tokens == null) return Redirect(SystemConfig.FE_DOMAIN + "/login");
            return Redirect(SystemConfig.FE_DOMAIN + "/redirect");
        }

        [HttpPost("signup")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SignUp([FromBody] UserSignUp userSignUp)
        {
            await createUserWorkflow.process(userSignUp);
            return NoContent();
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Refresh()
        {
            Tokens tokens = new Tokens
            {
                AccessToken = Request.Cookies["access_token"] ?? string.Empty,
                RefreshToken = Request.Cookies["refresh_token"] ?? string.Empty
            };
            if (ObjectUtils.IsEmpty(tokens.AccessToken) || ObjectUtils.IsEmpty(tokens.RefreshToken))
            {
                return BadRequest();
            }
            Tokens newTokens = await userService.RefreshAccessToken(tokens);
            tokenService.SetTokensInsideCookie(newTokens, HttpContext);
            return Ok();
        }

        [HttpGet("confirm-email")]
        [Authorize]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ConfirmEmail([FromQuery, Required] string access_token)
        {
            string email = User.FindFirst(ClaimEnum.EMAIL)?.Value ?? string.Empty;
            tokenService.SetTokensInsideCookie(await userService.ConfirmEmail(email), HttpContext);
            return Ok(); 
        }

        [HttpPost("resend-email")]
        [ProducesResponseType(typeof(ErrorObject), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ResendVerficationEmail(string email)
        {
            await userService.SendConfirmationEmail(email);
            return NoContent();
        }

        [HttpGet("decode")]
        [Authorize]
        public IActionResult Decode()
        {
            return Ok(currentUser);
        }
    }
}
