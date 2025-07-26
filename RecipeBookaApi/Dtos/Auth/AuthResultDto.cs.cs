using RecipeBookaApi.DA.Models;

namespace RecipeBookApi.Dtos.Auth
{
    public class AuthResultDto
    {

        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public TbUser User { get; set; }
    }
}
