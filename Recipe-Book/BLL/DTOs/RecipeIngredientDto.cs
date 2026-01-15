using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class RecipeIngredientDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string DisplayAmount { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
    public string? PrepNote { get; set; }
    public List<string> Allergens { get; set; } = new();
}

public class CreateRecipeIngredientDto
{
    public int IngredientId { get; set; }

    [Range(0.01, 10000)]
    public decimal Amount { get; set; }

    public int Unit { get; set; }
    public bool IsOptional { get; set; }
    public string? PrepNote { get; set; }
    public int SortOrder { get; set; }
}
