namespace BLL.DTOs;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DefaultUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public List<string> Allergens { get; set; } = new();
}
