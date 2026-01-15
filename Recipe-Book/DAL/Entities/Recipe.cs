using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Recipe
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(1, 500)]
    public int BaseServingSize { get; set; }

    [Range(0, 1440)]
    public int PrepTimeMinutes { get; set; }

    [Range(0, 1440)]
    public int CookTimeMinutes { get; set; }

    public DifficultyLevel DifficultyLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<RecipeCategory> RecipeCategories { get; set; } = new List<RecipeCategory>();
}