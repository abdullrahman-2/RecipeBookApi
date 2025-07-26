using RecipeBookApi.Dtos.Recipe;

namespace RecipeBookApi.Dtos.User
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public List<VwRecipeDto> vwRecipeDtos { get; set; }


    }
}
