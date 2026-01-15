using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class UpdateRecipeDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1, 100)]
    public int BaseServingSize { get; set; }

    [Required]
    [Range(0, 1440)]
    public int PrepTimeMinutes { get; set; }

    [Required]
    [Range(0, 1440)]
    public int CookTimeMinutes { get; set; }

    [Required]
    [Range(1, 3)]
    public int DifficultyLevel { get; set; }

    public List<int> CategoryIds { get; set; } = new();

    public List<CreateRecipeIngredientDto> Ingredients { get; set; } = new();
}
