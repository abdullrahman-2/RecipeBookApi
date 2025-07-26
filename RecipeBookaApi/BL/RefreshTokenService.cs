using Microsoft.EntityFrameworkCore;
using RecipeBookaApi.DA.Models;
using RecipeBookApi.DA.Models;

namespace RecipeBookApi.BL
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly RecipeBookContext _context;

        public RefreshTokenService(RecipeBookContext context)
        {
            _context = context;
        }

        public async Task SaveRefreshTokenAsync(TbRefreshToken refreshToken)
        {
            if (refreshToken.IsRevoked)
                return;

            _context.TbRefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<TbRefreshToken?> GetRefreshTokenAsync(string token)
        {
            
            return await _context.TbRefreshTokens
                                 .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            
            var refreshToken = await _context.TbRefreshTokens
                                             .FirstOrDefaultAsync(t => t.Token == token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;  
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOldUserRefreshTokensAsync(int userId)
        {
            var oldRefreshTokens = await _context.TbRefreshTokens
                                                .Where(t => t.UserId == userId && (t.IsRevoked || t.ExpiryTime <= DateTime.UtcNow))
                                                .ToListAsync();

            if (oldRefreshTokens.Any())
            {
                _context.TbRefreshTokens.RemoveRange(oldRefreshTokens);
                await _context.SaveChangesAsync();
            }
        }
    }

}

