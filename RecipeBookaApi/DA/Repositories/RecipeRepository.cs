using Microsoft.EntityFrameworkCore;
using RecipeBookaApi.DA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // مهم جداً عشان async/await

namespace RecipeBookApi.DA.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeBookContext _Context;
        public RecipeRepository(RecipeBookContext Context)
        {
            _Context = Context;
        }

        

        public async Task<bool> DeleteById(int id)
        {
            var deletedRecipe = await GetById(id); 
            if (deletedRecipe == null)
            {
                return false; 
            }

            deletedRecipe.CurrentState = 0; 
            _Context.Entry(deletedRecipe).State = EntityState.Modified;
            await _Context.SaveChangesAsync(); 
            return true;
        }

        public async Task<List<TbRecipe>> GetAll()
        {
            List<TbRecipe> recipes = await _Context.TbRecipes.ToListAsync(); 
            return recipes;
        }

        public async Task<TbRecipe?> GetById(int id) 
        {
            var recipe = await _Context.TbRecipes.Where(r => r.Id == id).FirstOrDefaultAsync(); 
            
            return recipe;
        }

        // للـ Add (حفظ وصفة جديدة)
        public async Task<bool> SaveRecipe(TbRecipe recipe)
        {
            _Context.TbRecipes.Add(recipe);
            await _Context.SaveChangesAsync(); 
            return true; 
        }

        // ميثود جديدة لعملية الـ Update
        public async Task<bool> UpdateRecipe(TbRecipe recipe) // هتستقبل الكائن الـ TbRecipe بعد ما تم تعديله
        {
            _Context.Entry(recipe).State = EntityState.Modified; // تعليم الـ Context إن الكائن ده تم تعديله
            // ممكن تستخدم _Context.TbRecipes.Update(recipe); برضه
            await _Context.SaveChangesAsync();
            return true; // لو وصلت هنا، يبقى العملية نجحت
        }
    }
}