using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebApp.Pages.Recipes;

public class DetailsModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IUnitConversionService _unitConversionService;
    private readonly IUserProfileService _userProfileService;
    private const string UnitSystemCookieName = "UnitSystem";

    public DetailsModel(
        IRecipeService recipeService,
        IUnitConversionService unitConversionService,
        IUserProfileService userProfileService)
    {
        _recipeService = recipeService;
        _unitConversionService = unitConversionService;
        _userProfileService = userProfileService;
    }

    public RecipeDetailDto Recipe { get; set; } = null!;
    public ScaledRecipeDto ScaledRecipe { get; set; } = null!;
    public bool UseImperialUnits { get; set; }
    public bool IsFavorite { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Servings { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            LoadUnitPreference();

            var recipe = await _recipeService.GetDetailByIdAsync(Id);
            if (recipe == null)
            {
                TempData["ErrorMessage"] = "Recipe not found.";
                return RedirectToPage("Index");
            }

            Recipe = recipe;
            var targetServings = Servings ?? Recipe.BaseServingSize;
            ScaledRecipe = await _recipeService.GetScaledRecipeAsync(Id, targetServings);

            ApplyUnitConversion();

            // Check if recipe is favorited (only for authenticated users)
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                IsFavorite = await _userProfileService.IsFavoriteAsync(userId, Id);
            }

            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load recipe. Please try again.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostToggleFavoriteAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            TempData["ErrorMessage"] = "Please login to add recipes to favorites.";
            return RedirectToPage("/Account/Login", new { returnUrl = $"/Recipes/Details?id={Id}" });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isFavorite = await _userProfileService.IsFavoriteAsync(userId, Id);

            if (isFavorite)
            {
                await _userProfileService.RemoveFavoriteRecipeAsync(userId, Id);
                TempData["SuccessMessage"] = "Recipe removed from favorites!";
            }
            else
            {
                await _userProfileService.AddFavoriteRecipeAsync(userId, Id);
                TempData["SuccessMessage"] = "Recipe added to favorites!";
            }

            return RedirectToPage(new { id = Id, servings = Servings });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to update favorites: {ex.Message}";
            return RedirectToPage(new { id = Id, servings = Servings });
        }
    }

    public async Task<IActionResult> OnGetScaleAsync(int servings)
    {
        try
        {
            LoadUnitPreference();
            var scaledRecipe = await _recipeService.GetScaledRecipeAsync(Id, servings);

            if (UseImperialUnits)
            {
                foreach (var ingredient in scaledRecipe.Ingredients)
                {
                    var unit = Enum.Parse<DAL.Entities.MeasurementUnit>(ingredient.ScaledUnit);
                    if (_unitConversionService.IsMetricUnit(unit))
                    {
                        ingredient.DisplayAmount = _unitConversionService.ConvertToImperial(ingredient.ScaledAmount, unit);
                    }
                }
            }

            return new JsonResult(scaledRecipe);
        }
        catch (Exception)
        {
            return new JsonResult(new { error = "Failed to scale recipe" }) { StatusCode = 500 };
        }
    }

    private void LoadUnitPreference()
    {
        UseImperialUnits = Request.Cookies.TryGetValue(UnitSystemCookieName, out var cookieValue)
            && cookieValue == "Imperial";
    }

    private void ApplyUnitConversion()
    {
        if (UseImperialUnits)
        {
            foreach (var ingredient in ScaledRecipe.Ingredients)
            {
                var unit = Enum.Parse<DAL.Entities.MeasurementUnit>(ingredient.ScaledUnit);
                if (_unitConversionService.IsMetricUnit(unit))
                {
                    ingredient.DisplayAmount = _unitConversionService.ConvertToImperial(ingredient.ScaledAmount, unit);
                }
            }
        }
    }
}
