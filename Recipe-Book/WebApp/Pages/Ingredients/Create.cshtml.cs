using BLL.DTOs;
using BLL.Exceptions;
using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Ingredients;

public class CreateModel : PageModel
{
    private readonly IIngredientService _ingredientService;
    private readonly IAllergenService _allergenService;

    public CreateModel(IIngredientService ingredientService, IAllergenService allergenService)
    {
        _ingredientService = ingredientService;
        _allergenService = allergenService;
    }

    [BindProperty]
    public CreateIngredientDto Input { get; set; } = new();

    public List<SelectListItem> MeasurementUnits { get; set; } = new();
    public List<SelectListItem> IngredientCategories { get; set; } = new();
    public List<SelectListItem> AvailableAllergens { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
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
            var created = await _ingredientService.CreateAsync(Input);
            TempData["SuccessMessage"] = $"Ingredient '{created.Name}' created successfully!";
            return RedirectToPage("Index");
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
            ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the ingredient.");
            TempData["ErrorMessage"] = "Failed to create ingredient. Please try again.";
            await LoadSelectListsAsync();
            return Page();
        }
    }

    private async Task LoadSelectListsAsync()
    {
        MeasurementUnits = Enum.GetValues<MeasurementUnit>()
            .Select(u => new SelectListItem
            {
                Value = ((int)u).ToString(),
                Text = u.ToString()
            })
            .ToList();

        IngredientCategories = Enum.GetValues<IngredientCategory>()
            .Select(c => new SelectListItem
            {
                Value = ((int)c).ToString(),
                Text = c.ToString()
            })
            .ToList();

        var allergens = await _allergenService.GetAllAsync();
        AvailableAllergens = allergens
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            })
            .ToList();
    }
}
