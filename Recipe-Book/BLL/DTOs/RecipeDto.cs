namespace BLL.DTOs;

public class RecipeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BaseServingSize { get; set; }
    public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public bool HasAllergens { get; set; }
    public List<string> AllergenWarnings { get; set; } = new();
}
