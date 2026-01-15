namespace BLL.DTOs;

public class ScaledRecipeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OriginalServings { get; set; }
    public int ScaledServings { get; set; }
    public decimal ScaleFactor { get; set; }
    public List<ScaledIngredientDto> Ingredients { get; set; } = new();
    public bool HasScalingWarning { get; set; }
    public string? ScalingWarningMessage { get; set; }
}

public class ScaledIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public string OriginalUnit { get; set; } = string.Empty;
    public decimal ScaledAmount { get; set; }
    public string ScaledUnit { get; set; } = string.Empty;
    public string DisplayAmount { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
    public string? PrepNote { get; set; }
}
