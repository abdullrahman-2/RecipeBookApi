using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RecipeBookaApi.DA.Models;
using RecipeBookApi.DA.Models;

namespace RecipeBookApi.Helper
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<TbUser> _userManager;
       

        public JwtTokenGenerator(IConfiguration configuration, UserManager<TbUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessToken(TbUser user)
        {
            var jwtSetting = _configuration.GetSection("JwtSettings");
            var Issuer = jwtSetting["Issuer"];
            var Audience = jwtSetting["Audience"];
            var Expritation = double.Parse (jwtSetting["TokenExpirationMinutes"]);
            var SecretKey = jwtSetting["Secret"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

            var Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claim = new List<Claim>() {
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
    new Claim(ClaimTypes.Name,user.UserName),
    new Claim(ClaimTypes.Email,user.Email),
};

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role)); 
            }

            var token = new JwtSecurityToken(
                 issuer: Issuer,
                 audience: Audience,
                 claims: claim,
                 expires: DateTime.UtcNow.AddMinutes(Expritation),
                 signingCredentials: Credentials
                );
            
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<TbRefreshToken> GenerateRefreshToken(TbUser user)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            string refreshTokenString = Convert.ToBase64String(randomNumber);

            var jwtSettings = _configuration.GetSection("JwtSettings");
  
            var RefreshTokenExpirationDays = double.Parse(jwtSettings["RefreshTokenExpirationDays"]);

            var refreshTokenEntity = new TbRefreshToken
            {
                Token = refreshTokenString,
                ExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
                UserId = user.Id, 
                IsRevoked = false 
            };

            return refreshTokenEntity; 
        }
    }
}
