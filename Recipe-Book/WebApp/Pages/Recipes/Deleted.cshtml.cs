using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Recipes;

public class DeletedModel : PageModel
{
    private readonly IRecipeService _recipeService;

    public DeletedModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public IEnumerable<RecipeDto> DeletedRecipes { get; set; } = new List<RecipeDto>();

    public async Task OnGetAsync()
    {
        try
        {
            DeletedRecipes = await _recipeService.GetDeletedAsync();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load deleted recipes.";
        }
    }

    public async Task<IActionResult> OnPostRestoreAsync(int id)
    {
        try
        {
            await _recipeService.RestoreAsync(id);
            TempData["SuccessMessage"] = "Recipe restored successfully!";
            return RedirectToPage();
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to restore recipe. Please try again.";
            return RedirectToPage();
        }
    }
}
