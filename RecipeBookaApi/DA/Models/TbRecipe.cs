using System;
using System.Collections.Generic;

namespace RecipeBookaApi.DA.Models;

public partial class TbRecipe
{
    public string Name { get; set; } = null!;

    public string Ingredients { get; set; } = null!;
    public string Instructions { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Img { get; set; }

    public int CurrentState { get; set; }

    public DateTime? CreationDate { get; set; }

    public int Id { get; set; }

    public int UserId { get; set; }

    public  string UserName { get; set; } = null!;
    public virtual TbUser User { get; set; } = null!;
}
