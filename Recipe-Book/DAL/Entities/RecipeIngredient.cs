using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    [Range(0.01, 10000)]
    public decimal Amount { get; set; }

    public MeasurementUnit Unit { get; set; }

    public bool IsOptional { get; set; }

    [MaxLength(200)]
    public string? PrepNote { get; set; }

    public int SortOrder { get; set; }
}
