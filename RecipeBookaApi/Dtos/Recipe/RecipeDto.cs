using System.ComponentModel.DataAnnotations;

namespace RecipeBookApi.Dtos.Recipe;

public class RecipeDto
{
    public int id { get; set; }

    [Required(ErrorMessage = "Recipe name is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Recipe name must be between 3 and 255 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Ingredients are required.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Ingredients must be at least 10 characters long.")]
    public string Ingredients { get; set; } = null!;

    [Required(ErrorMessage = "Instructions are required.")]
    public string Instructions { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;
    public IFormFile? ImageFile { get; set; } // استخدم ? لجعلها اختيارية (لو الوصفة ممكن تكون من غير صورة)

    public int CurrentState { get; set; }
    public DateTime? CreationDate { get; set; }

    public string? Img { get; set; } 
}