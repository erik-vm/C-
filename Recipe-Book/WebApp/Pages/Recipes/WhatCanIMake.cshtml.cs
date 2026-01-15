using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Recipes;

public class WhatCanIMakeModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;

    public WhatCanIMakeModel(
        IRecipeService recipeService,
        IIngredientService ingredientService)
    {
        _recipeService = recipeService;
        _ingredientService = ingredientService;
    }

    [BindProperty(SupportsGet = true)]
    public List<int> AvailableIngredients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int MinMatchPercentage { get; set; } = 70;

    public List<SelectListItem> AllIngredients { get; set; } = new();
    public List<WhatCanIMakeDto> Results { get; set; } = new();
    public bool SearchPerformed { get; set; }

    public async Task OnGetAsync()
    {
        await LoadIngredientsAsync();

        if (AvailableIngredients.Any())
        {
            SearchPerformed = true;
            var results = await _recipeService.WhatCanIMakeAsync(AvailableIngredients, MinMatchPercentage);
            Results = results.ToList();
        }
    }

    private async Task LoadIngredientsAsync()
    {
        var ingredients = await _ingredientService.GetAllAsync();
        AllIngredients = ingredients
            .OrderBy(i => i.Name)
            .Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name,
                Selected = AvailableIngredients.Contains(i.Id)
            })
            .ToList();
    }
}
