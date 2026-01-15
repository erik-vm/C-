using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Ingredients;

public class IndexModel : PageModel
{
    private readonly IIngredientService _ingredientService;

    public IndexModel(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public IEnumerable<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();

    public async Task OnGetAsync()
    {
        Ingredients = await _ingredientService.GetAllAsync();
    }
}
