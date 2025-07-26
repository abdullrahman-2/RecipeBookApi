using RecipeBookaApi.DA.Models;  
using RecipeBookaApi.Dtos.User;
 using RecipeBookApi.Dtos.Auth;
using RecipeBookApi.Dtos.User;
namespace RecipeBookApi.BL
{
    public interface IUserService
    {
        public Task<AuthResultDto> RegisterUserAsync(RegisterUserDto model);

         public Task<AuthResultDto> LoginUserAsync(LoginUserDto model);

        Task<TbUser?> GetUserByIdAsync(string userId); 

        public Task<UserDto?> UpdateUserAsync(UpdateUserDto userDto);

         public Task<bool> DeleteUserAsync(int id);

        public Task<UserDto> GetUserInfoAsync(string Userid);
        public Task<bool> DeleteRecipesByUserIdAsync(int userId);
    }
}