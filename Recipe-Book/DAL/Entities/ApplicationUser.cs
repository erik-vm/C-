using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserAllergenProfile> UserAllergenProfiles { get; set; } = new List<UserAllergenProfile>();
    public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();
}
