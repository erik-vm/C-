using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class CreateRecipeDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(1, 500)]
    public int BaseServingSize { get; set; }

    [Range(0, 1440)]
    public int PrepTimeMinutes { get; set; }

    [Range(0, 1440)]
    public int CookTimeMinutes { get; set; }

    public int DifficultyLevel { get; set; }

    public List<int> CategoryIds { get; set; } = new();

    public List<CreateRecipeIngredientDto> Ingredients { get; set; } = new();
}
