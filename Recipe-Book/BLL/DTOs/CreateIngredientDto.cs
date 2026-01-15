using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class CreateIngredientDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int DefaultUnit { get; set; }

    [Required]
    public int Category { get; set; }

    public bool IsVegetarian { get; set; }

    public bool IsVegan { get; set; }

    public List<int> AllergenIds { get; set; } = new();
}
