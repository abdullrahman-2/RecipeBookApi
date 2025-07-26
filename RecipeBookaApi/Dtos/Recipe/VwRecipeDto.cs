namespace RecipeBookApi.Dtos.Recipe
{
    public class VwRecipeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Ingredients { get; set; } = null!;
        public string Instructions { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Img { get; set; } 
        public DateTime CreationDate { get; set; }
        public int CurrentState { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
    }
}
