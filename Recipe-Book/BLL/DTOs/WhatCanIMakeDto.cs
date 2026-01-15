namespace BLL.DTOs;

public class WhatCanIMakeDto
{
    public int RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BaseServingSize { get; set; }
    public int TotalTimeMinutes { get; set; }
    public string DifficultyLevel { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public int MatchPercentage { get; set; }
    public int RequiredIngredientsCount { get; set; }
    public int MatchedIngredientsCount { get; set; }
    public List<string> MissingIngredients { get; set; } = new();
    public List<string> MatchedIngredients { get; set; } = new();
    public bool HasAllergens { get; set; }
    public List<string> AllergenWarnings { get; set; } = new();
}
