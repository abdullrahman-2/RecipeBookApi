 using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;
 using RecipeBookApi.ApiForm;
using RecipeBookApi.BL;
using RecipeBookApi.Dtos.Recipe;
using System.Security.Claims;

namespace RecipeBookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService) 
        {
            _recipeService = recipeService;
           
        }

        [HttpGet]
       
     [AllowAnonymous]
        public async Task<IActionResult> GetAllRecipe()
           {
            var data = await _recipeService.GetAllRecipesAsync();

            return Ok(ApiResponse<List<VwRecipeDto>>.SuccessResponse(data, "All recipes retrieved successfully."));

        }

        [HttpGet("GetUserRecipes")]
   
        public async Task<IActionResult> GetUserRecipes()
        {
         
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            if (string.IsNullOrEmpty(userId))
            {
                
                return Unauthorized(ApiResponse<List<VwRecipeDto>>.ErrorResponse("User not authenticated or ID not found."));
            }

            var data = await _recipeService.GetUserRecipesAsync(userId);

            return Ok(ApiResponse<List<VwRecipeDto>>.SuccessResponse(data, "User recipes retrieved successfully."));
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecipeById(int id)
        {

            var data = await _recipeService.GetRecipeByIdAsync(id);
            if (data == null)
                return NotFound(ApiResponse<VwRecipeDto>.ErrorResponse($"Recipe With ID = {id} is not found"));

            return Ok(ApiResponse<VwRecipeDto>.SuccessResponse(data));

        }
        [HttpPost]

        public async Task<IActionResult> CreateRecipe([FromForm] RecipeDto recipe) {


        recipe =  await  _recipeService.MakeImage(recipe);


            var data = await _recipeService.CreateRecipeAsync(recipe, User.FindFirstValue(ClaimTypes.NameIdentifier));

        
            if (data == null)
                return BadRequest(ApiResponse<RecipeDto>.ErrorResponse("Failed to create recipe or user not found."));
            return CreatedAtAction(nameof(GetRecipeById), new { id = data.id }, ApiResponse<RecipeDto>.SuccessResponse(data, "Recipe created successfully!"));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromForm] RecipeDto recipe)
        {
            

            recipe = await _recipeService.MakeImage(recipe);


            var updateResult = await _recipeService.UpdateRecipeAsync(recipe);
            if (!updateResult) 
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to update recipe or recipe not found."));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Recipe updated successfully!"));
        }

        [HttpDelete("DeleteRecipe")]
        public async Task<IActionResult> DeleteRecipe( [FromBody] int id) { 
        
            var data = await _recipeService.DeleteRecipeAsync(id);
            if (!data) 
            {
             
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to delete recipe or recipe not found."));
            }

             return Ok(ApiResponse<bool>.SuccessResponse(true, "Recipe deleted successfully!"));

        }




    }
}
