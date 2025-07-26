using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookApi.ApiForm;
using RecipeBookApi.BL;
using RecipeBookApi.Dtos.Auth;
using RecipeBookApi.Dtos.User;
using RecipeBookApi.Helper;

namespace RecipeBookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthController(
            IJwtTokenGenerator jwtTokenGenerator,
            IUserService userService,
            IRefreshTokenService refreshTokenService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTimeOffset expiryTime)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiryTime,
                Secure = false,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private void DeleteRefreshTokenCookie()
        {
            Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            var registerResult = await _userService.RegisterUserAsync(userDto);

            if (!registerResult.Success)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(
                    "User registration failed.",
                    registerResult.Errors?.ToList()
                ));
            }

            var registeredUser = registerResult.User;

            if (registeredUser == null)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse(
                    "An internal error occurred: User data not found after registration.",
                    new List<string> { "User object was null after successful registration." }
                ));
            }

            var accessToken = await _jwtTokenGenerator.GenerateAccessToken(registeredUser);
            var refreshTokenEntity = await _jwtTokenGenerator.GenerateRefreshToken(registeredUser);

            await _refreshTokenService.SaveRefreshTokenAsync(refreshTokenEntity);
            await _refreshTokenService.DeleteOldUserRefreshTokensAsync(registeredUser.Id);

            SetRefreshTokenCookie(refreshTokenEntity.Token, refreshTokenEntity.ExpiryTime);

            var authResponse = new AuthResponseDto
            {
                Id = registeredUser.Id,
                UserName = registeredUser.UserName,
                Email = registeredUser.Email,
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                RefreshTokenExpiry = refreshTokenEntity.ExpiryTime
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    authResponse,
                    "User registered successfully."
                ));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginDto)
        {
            var loginResult = await _userService.LoginUserAsync(loginDto);

            if (!loginResult.Success)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Login failed.",
                    loginResult.Errors?.ToList() ?? new List<string> { "Invalid username/email or password." }
                ));
            }

            var user = loginResult.User;

            if (user == null)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse(
                    "An internal error occurred: User data not found after login.",
                    new List<string> { "User object was null after successful login." }
                ));
            }

            var accessToken = await _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshTokenEntity = await _jwtTokenGenerator.GenerateRefreshToken(user);

            await _refreshTokenService.SaveRefreshTokenAsync(refreshTokenEntity);
            await _refreshTokenService.DeleteOldUserRefreshTokensAsync(user.Id);

            SetRefreshTokenCookie(refreshTokenEntity.Token, refreshTokenEntity.ExpiryTime);

            var authResponse = new AuthResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                RefreshTokenExpiry = refreshTokenEntity.ExpiryTime
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    authResponse,
                    "User logged in successfully."
                ));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Ok(ApiResponse<object>.SuccessResponse(null, "Already logged out or no active session."));
            }

            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

            DeleteRefreshTokenCookie();

            return Ok(ApiResponse<object>.SuccessResponse(null, "Logged out successfully."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Refresh token is required."));
            }

            var storedRefreshToken = await _refreshTokenService.GetRefreshTokenAsync(refreshToken);

            if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiryTime <= DateTime.UtcNow)
            {
                DeleteRefreshTokenCookie();
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid or expired refresh token."));
            }

            var user = await _userService.GetUserByIdAsync(storedRefreshToken.UserId.ToString());

            if (user == null)
            {
                DeleteRefreshTokenCookie();
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("User associated with refresh token not found."));
            }

            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

            var newAccessToken = await _jwtTokenGenerator.GenerateAccessToken(user);
            var newRefreshTokenEntity = await _jwtTokenGenerator.GenerateRefreshToken(user);

            await _refreshTokenService.SaveRefreshTokenAsync(newRefreshTokenEntity);

            SetRefreshTokenCookie(newRefreshTokenEntity.Token, newRefreshTokenEntity.ExpiryTime);

            await _refreshTokenService.DeleteOldUserRefreshTokensAsync(user.Id);

            var authResponse = new AuthResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AccessToken = newAccessToken,

            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                authResponse,
                "Token refreshed successfully."
            ));
        }
    }
}