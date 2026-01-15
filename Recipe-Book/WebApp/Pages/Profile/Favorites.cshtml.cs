using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebApp.Pages.Profile;

[Authorize]
public class FavoritesModel : PageModel
{
    private readonly IUserProfileService _userProfileService;

    public FavoritesModel(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public List<RecipeDto> FavoriteRecipes { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            FavoriteRecipes = await _userProfileService.GetFavoriteRecipesAsync(userId);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load favorite recipes.";
        }
    }

    public async Task<IActionResult> OnPostRemoveFavoriteAsync(int recipeId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _userProfileService.RemoveFavoriteRecipeAsync(userId, recipeId);
            TempData["SuccessMessage"] = "Recipe removed from favorites!";
            return RedirectToPage();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to remove recipe from favorites.";
            return RedirectToPage();
        }
    }
}
