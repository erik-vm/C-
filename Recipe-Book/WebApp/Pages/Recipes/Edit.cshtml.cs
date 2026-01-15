using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Recipes;

public class EditModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;
    private readonly ICategoryService _categoryService;

    public EditModel(
        IRecipeService recipeService,
        IIngredientService ingredientService,
        ICategoryService categoryService)
    {
        _recipeService = recipeService;
        _ingredientService = ingredientService;
        _categoryService = categoryService;
    }

    [BindProperty]
    public UpdateRecipeDto Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public List<SelectListItem> AvailableIngredients { get; set; } = new();
    public List<SelectListItem> AvailableCategories { get; set; } = new();
    public List<SelectListItem> DifficultyLevels { get; set; } = new();
    public List<SelectListItem> MeasurementUnits { get; set; } = new();
    public Dictionary<int, string> IngredientUnits { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var recipe = await _recipeService.GetDetailByIdAsync(Id);
            if (recipe == null)
            {
                TempData["ErrorMessage"] = "Recipe not found.";
                return RedirectToPage("Index");
            }

            // Convert string difficulty level to int (Easy=1, Medium=2, Hard=3)
            var difficultyLevel = recipe.DifficultyLevel switch
            {
                "Easy" => 1,
                "Medium" => 2,
                "Hard" => 3,
                _ => 1
            };

            Input = new UpdateRecipeDto
            {
                Name = recipe.Name,
                Description = recipe.Description,
                BaseServingSize = recipe.BaseServingSize,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                DifficultyLevel = difficultyLevel,
                CategoryIds = await GetCategoryIdsByNamesAsync(recipe.Categories),
                Ingredients = recipe.Ingredients.Select((i, index) => new CreateRecipeIngredientDto
                {
                    IngredientId = i.IngredientId,
                    Amount = i.Amount,
                    Unit = (int)Enum.Parse<DAL.Entities.MeasurementUnit>(i.Unit),
                    IsOptional = i.IsOptional,
                    PrepNote = i.PrepNote ?? string.Empty,
                    SortOrder = index
                }).ToList()
            };

            await LoadSelectListsAsync();
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
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        try
        {
            var updated = await _recipeService.UpdateAsync(Id, Input);
            TempData["SuccessMessage"] = $"Recipe '{updated.Name}' updated successfully!";
            return RedirectToPage("Details", new { id = Id });
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
            ModelState.AddModelError(string.Empty, "An unexpected error occurred while updating the recipe.");
            TempData["ErrorMessage"] = "Failed to update recipe. Please try again.";
            await LoadSelectListsAsync();
            return Page();
        }
    }

    private async Task<List<int>> GetCategoryIdsByNamesAsync(List<string> categoryNames)
    {
        var allCategories = await _categoryService.GetAllAsync();
        return allCategories
            .Where(c => categoryNames.Contains(c.Name))
            .Select(c => c.Id)
            .ToList();
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
