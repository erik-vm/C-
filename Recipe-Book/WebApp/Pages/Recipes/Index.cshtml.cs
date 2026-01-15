using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Recipes;

public class IndexModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly ICategoryService _categoryService;
    private readonly IIngredientService _ingredientService;
    private readonly IAllergenService _allergenService;

    public IndexModel(
        IRecipeService recipeService,
        ICategoryService categoryService,
        IIngredientService ingredientService,
        IAllergenService allergenService)
    {
        _recipeService = recipeService;
        _categoryService = categoryService;
        _ingredientService = ingredientService;
        _allergenService = allergenService;
    }

    public PagedResultDto<RecipeDto> PagedResult { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<int> CategoryIds { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public List<int> DifficultyLevels { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? MaxTotalTime { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<int> MustContainIngredients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public List<int> MustNotContainIngredients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public List<int> ExcludeAllergens { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public bool VegetarianOnly { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool VeganOnly { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 25;

    public List<SelectListItem> AvailableCategories { get; set; } = new();
    public List<SelectListItem> AvailableDifficulties { get; set; } = new();
    public List<SelectListItem> AvailableIngredients { get; set; } = new();
    public List<SelectListItem> AvailableAllergens { get; set; } = new();
    public List<SelectListItem> PageSizeOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        LoadAllergenProfileFromCookie();
        await LoadFiltersAsync();

        var searchDto = new RecipeSearchDto
        {
            SearchTerm = SearchTerm,
            CategoryIds = CategoryIds,
            DifficultyLevels = DifficultyLevels,
            MaxTotalTimeMinutes = MaxTotalTime,
            MustContainIngredientIds = MustContainIngredients,
            MustNotContainIngredientIds = MustNotContainIngredients,
            ExcludeAllergenIds = ExcludeAllergens,
            VegetarianOnly = VegetarianOnly ? true : null,
            VeganOnly = VeganOnly ? true : null,
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        PagedResult = await _recipeService.SearchAsync(searchDto);
    }

    private void LoadAllergenProfileFromCookie()
    {
        const string cookieName = "AllergenProfile";
        if (Request.Cookies.TryGetValue(cookieName, out var cookieValue) && !ExcludeAllergens.Any())
        {
            var allergenIds = cookieValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
            ExcludeAllergens = allergenIds;
        }
    }

    private async Task LoadFiltersAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        AvailableCategories = categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = CategoryIds.Contains(c.Id)
            })
            .ToList();

        AvailableDifficulties = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Easy", Selected = DifficultyLevels.Contains(1) },
            new SelectListItem { Value = "2", Text = "Medium", Selected = DifficultyLevels.Contains(2) },
            new SelectListItem { Value = "3", Text = "Hard", Selected = DifficultyLevels.Contains(3) }
        };

        var ingredients = await _ingredientService.GetAllAsync();
        AvailableIngredients = ingredients
            .Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name
            })
            .ToList();

        var allergens = await _allergenService.GetAllAsync();
        AvailableAllergens = allergens
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name,
                Selected = ExcludeAllergens.Contains(a.Id)
            })
            .ToList();

        PageSizeOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "10", Text = "10 per page", Selected = PageSize == 10 },
            new SelectListItem { Value = "25", Text = "25 per page", Selected = PageSize == 25 },
            new SelectListItem { Value = "50", Text = "50 per page", Selected = PageSize == 50 }
        };
    }
}
