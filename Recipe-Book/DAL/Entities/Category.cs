using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
}
