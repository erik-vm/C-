using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IRecipeService _recipeService;

    public IndexModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public IEnumerable<RecipeDto> Recipes { get; set; } = new List<RecipeDto>();

    public async Task OnGetAsync()
    {
        Recipes = await _recipeService.GetAllAsync();
    }
}
