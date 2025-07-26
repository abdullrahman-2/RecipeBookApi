using RecipeBookaApi.DA.Models;
using RecipeBookApi.DA.Models;

namespace RecipeBookApi.Helper
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateAccessToken(TbUser user);

        Task<TbRefreshToken> GenerateRefreshToken(TbUser user);
    }
}
