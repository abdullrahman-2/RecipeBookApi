 using RecipeBookApi.Dtos.Recipe;  

namespace RecipeBookApi.BL
{
    public interface IRecipeService
    {
         public Task<RecipeDto> MakeImage(RecipeDto recipe);
        public Task<VwRecipeDto> GetRecipeByIdAsync(int id);
        
         public Task<List<VwRecipeDto>> GetAllRecipesAsync();

         public Task<List<VwRecipeDto>> GetUserRecipesAsync(string userId);

         public Task<RecipeDto?> CreateRecipeAsync(RecipeDto recipeDto, string userId);

         public Task<bool> UpdateRecipeAsync(RecipeDto recipeDto);

         public Task<bool> DeleteRecipeAsync(int id);
    }
}