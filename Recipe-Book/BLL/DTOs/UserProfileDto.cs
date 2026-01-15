namespace BLL.DTOs;

public class UserProfileDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<int> AllergenProfileIds { get; set; } = new();
    public List<string> AllergenNames { get; set; } = new();
    public int FavoriteRecipesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
