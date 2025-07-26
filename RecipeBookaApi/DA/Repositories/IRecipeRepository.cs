using RecipeBookaApi.DA.Models;

namespace RecipeBookApi.DA.Repositories
{
    public interface IRecipeRepository
    {
        public Task<TbRecipe> GetById(int id);
        public Task<List<TbRecipe>> GetAll();

        public Task<bool> DeleteById(int id);
        public Task<bool> UpdateRecipe(TbRecipe recipe);
        public Task<bool> SaveRecipe(TbRecipe recipe);



    }
}
