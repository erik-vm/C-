using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Recipes;

public class CreateModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private readonly ICategoryService _categoryService;

    public CreateModel(
        IRecipeService recipeService,
        IIngredientService ingredientService,
        ICategoryService categoryService)
    {
        _recipeService = recipeService;
        _ingredientService = ingredientService;
        _categoryService = categoryService;
    }

    [BindProperty]
    public CreateRecipeDto Input { get; set; } = new();

    public List<SelectListItem> AvailableIngredients { get; set; } = new();
    public List<SelectListItem> AvailableCategories { get; set; } = new();
    public List<SelectListItem> DifficultyLevels { get; set; } = new();
    public List<SelectListItem> MeasurementUnits { get; set; } = new();
    public Dictionary<int, string> IngredientUnits { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
        Input.BaseServingSize = 4;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        try
        {
            var created = await _recipeService.CreateAsync(Input);
            TempData["SuccessMessage"] = $"Recipe '{created.Name}' created successfully!";
            return RedirectToPage("Details", new { id = created.Id });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            TempData["ErrorMessage"] = "Please fix the validation errors below.";
            await LoadSelectListsAsync();
            return Page();
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData["ErrorMessage"] = ex.Message;
            await LoadSelectListsAsync();
            return Page();
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the recipe.");
            TempData["ErrorMessage"] = "Failed to create recipe. Please try again.";
            await LoadSelectListsAsync();
            return Page();
        }
    }

    private async Task LoadSelectListsAsync()
    {
        var ingredients = await _ingredientService.GetAllAsync();
        AvailableIngredients = ingredients
            .Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = $"{i.Name} ({i.DefaultUnit})"
            })
            .ToList();

        IngredientUnits = ingredients.ToDictionary(
            i => i.Id,
            i => ((int)Enum.Parse<DAL.Entities.MeasurementUnit>(i.DefaultUnit)).ToString()
        );

        var categories = await _categoryService.GetAllAsync();
        AvailableCategories = categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();

        DifficultyLevels = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Easy" },
            new SelectListItem { Value = "2", Text = "Medium" },
            new SelectListItem { Value = "3", Text = "Hard" }
        };

        MeasurementUnits = Enum.GetValues<DAL.Entities.MeasurementUnit>()
            .Select(u => new SelectListItem
            {
                Value = ((int)u).ToString(),
                Text = u.ToString()
            })
            .ToList();
    }
}
