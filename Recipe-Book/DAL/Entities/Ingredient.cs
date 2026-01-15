using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Ingredient
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public MeasurementUnit DefaultUnit { get; set; }

    public IngredientCategory Category { get; set; }

    public bool IsVegetarian { get; set; }

    public bool IsVegan { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<IngredientAllergen> IngredientAllergens { get; set; } = new List<IngredientAllergen>();
}
