namespace BLL.DTOs;

public class RecipeSearchDto
{
    public string? SearchTerm { get; set; }

    public List<int> CategoryIds { get; set; } = new();

    public List<int> DifficultyLevels { get; set; } = new();

    public int? MaxTotalTimeMinutes { get; set; }

    public List<int> MustContainIngredientIds { get; set; } = new();

    public List<int> MustNotContainIngredientIds { get; set; } = new();

    public List<int> ExcludeAllergenIds { get; set; } = new();

    public bool? VegetarianOnly { get; set; }

    public bool? VeganOnly { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 25;
}
