using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonAPI.Models;
using PokemonAPI.Models.Response;
using PokemonAPI.Repository;

namespace PokemonAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthRepository _AuthRepository;
        public AuthenticateController(IAuthRepository AuthRepository)
        {
            _AuthRepository = AuthRepository;
        }

        [AllowAnonymous]
        [HttpPost("issue")]
        public async Task<IActionResult> Issue(UserAuthen userAuthen)
        {
            if (userAuthen is null)
            {
                return BadRequest("Invalid client request");
            }
            else
            {
                var response = await _AuthRepository.IssueToken(userAuthen);
                if (response != null)
                {
                    if (response.IsAuthSuccessful)
                    {
                        return Ok(new AuthResponse
                        {
                            IsAuthSuccessful = response.IsAuthSuccessful,
                            UserInfo = response.UserInfo,
                            Token = response.Token,
                            RefreshToken = response.RefreshToken
                        });
                    }
                }
                return Ok(new AuthResponse
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid Authentication"
                });

            }
        }
        [AllowAnonymous]
        [HttpPost("authenAzureAD")]
        public async Task<IActionResult> IssueAzureAD(UserAuthenMSAL userAuthen)
        {
            if (userAuthen is null)
            {
                return BadRequest("Invalid client request");
            }
            else
            {
                var response = await _AuthRepository.IssueTokenAzureAD(userAuthen);

                if (response.IsAuthSuccessful)
                {

                    return Ok(new AuthResponse
                    {
                        IsAuthSuccessful = response.IsAuthSuccessful,
                        UserInfo = response.UserInfo,
                        Token = response.Token,
                        RefreshToken = response.RefreshToken
                    });
                }

                return Ok(new AuthResponse
                {
                    IsAuthSuccessful = response.IsAuthSuccessful,
                    ErrorMessage = "Invalid Authentication"
                });

            }


        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync(TokenApi tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");


            var response = await _AuthRepository.RefreshToken(tokenApiModel);
            if (response.IsAuthSuccessful)
            {
                return Ok(new AuthResponse
                {
                    IsAuthSuccessful = response.IsAuthSuccessful,
                    UserInfo = response.UserInfo,
                    Token = response.Token,
                    RefreshToken = response.RefreshToken
                });
            }

            return Unauthorized(new AuthResponse
            {
                IsAuthSuccessful = response.IsAuthSuccessful,
                ErrorMessage = "Invalid Authentication"
            });

        }

        [HttpPost("revoke"), Authorize]

        public async Task<IActionResult> RevokeTokenAsync([FromBody] UserAuthen userAuthen)
        {
            var username = userAuthen.Username;
            if (string.IsNullOrWhiteSpace(username))
                username = User.Identity.Name; //this is mapped to the Name claim by default 

            userAuthen.Username = username;
            var response = await _AuthRepository.RevokeToken(userAuthen);

            return Ok(response);


        }
        [AllowAnonymous]
        [HttpPost("Forgot")]
        public async Task<ActionResult> Forgot([FromBody] UserForgot userForgot) => Ok(await _AuthRepository.ForgotPass(userForgot));
        [AllowAnonymous]
        [HttpPost("Reset")]
        public async Task<ActionResult> Reset([FromBody] UserReset userReset) => Ok(await _AuthRepository.ResetPass(userReset));

    }
}
