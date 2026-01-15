namespace BLL.DTOs;

public class RecipeDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionHtml { get; set; } = string.Empty;
    public int BaseServingSize { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;
    public string DifficultyLevel { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
