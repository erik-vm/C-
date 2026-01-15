using BLL.DTOs;

namespace BLL.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetUserProfileAsync(string userId);
    Task UpdateAllergenProfileAsync(string userId, List<int> allergenIds);
    Task<List<int>> GetUserAllergenProfileAsync(string userId);
    Task AddFavoriteRecipeAsync(string userId, int recipeId);
    Task RemoveFavoriteRecipeAsync(string userId, int recipeId);
    Task<bool> IsFavoriteAsync(string userId, int recipeId);
    Task<List<RecipeDto>> GetFavoriteRecipesAsync(string userId);
}
