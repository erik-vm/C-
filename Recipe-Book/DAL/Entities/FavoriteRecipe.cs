namespace DAL.Entities;

public class FavoriteRecipe
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RecipeId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;
    public Recipe? Recipe { get; set; }
}
