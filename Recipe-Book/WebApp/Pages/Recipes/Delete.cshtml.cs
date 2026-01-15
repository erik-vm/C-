using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Recipes;

public class DeleteModel : PageModel
{
    private readonly IRecipeService _recipeService;

    public DeleteModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public RecipeDto Recipe { get; set; } = null!;

    [BindProperty]
    public int Id { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
            {
                TempData["ErrorMessage"] = "Recipe not found.";
                return RedirectToPage("Index");
            }

            Recipe = recipe;
            Id = id;
            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load recipe. Please try again.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _recipeService.DeleteAsync(Id);
            TempData["SuccessMessage"] = "Recipe deleted successfully!";
            return RedirectToPage("Index");
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage("Details", new { id = Id });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to delete recipe. Please try again.";
            return RedirectToPage("Details", new { id = Id });
        }
    }
}
