using RecipeBookApi.DA.Models;

namespace RecipeBookApi.BL
{
    public interface IRefreshTokenService
    {
        Task SaveRefreshTokenAsync(TbRefreshToken refreshToken);
        Task<TbRefreshToken?> GetRefreshTokenAsync(string token); 
        Task RevokeRefreshTokenAsync(string token); 
        Task DeleteOldUserRefreshTokensAsync(int userId); 

    }
}
