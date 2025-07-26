// في Controllers/UserController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeBookApi.ApiForm;
using RecipeBookApi.BL;
using RecipeBookApi.Dtos.User;
using System.Security.Claims;  
 using Microsoft.AspNetCore.Authorization;
using RecipeBookaApi.Dtos.User;  

namespace RecipeBookApi.Controllers
{
    [Route("api/[controller]")]  
    [ApiController]
     [Authorize]  
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
      

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

     
        [HttpGet("ShowUserInfo")] 
        public async Task<IActionResult> GetUserInfo()
        {
             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            
                return Unauthorized(ApiResponse<UserDto>.ErrorResponse("Unauthorized: User ID not found in token."));
            

            var user = await _userService.GetUserInfoAsync(userId);  

            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found."));  
            }

            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User information retrieved successfully."));
        }


        [HttpPut("UpdateUser")] // Path: api/User/me
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _userService.UpdateUserAsync( updateDto);
            if (result ==null) return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update user."));

            return Ok(ApiResponse<object>.SuccessResponse(null, "User updated successfully."));
        }

        [HttpDelete("DeleteUser")] 
         public async Task<IActionResult> DeleteCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _userService.DeleteUserAsync(int.Parse(userId));
            if (!result) return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete user."));

            return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully."));
        }
    }
}