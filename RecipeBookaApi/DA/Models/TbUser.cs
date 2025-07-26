
using Microsoft.AspNetCore.Identity; 

namespace RecipeBookaApi.DA.Models;

public partial class TbUser : IdentityUser<int>
{
   
    public int CurrentState { get; set; }

    public virtual ICollection<TbRecipe> tbRecipes { get; set; } = new List<TbRecipe>();

    public TbUser()
    {
        tbRecipes = new List<TbRecipe>(); 
    }
}