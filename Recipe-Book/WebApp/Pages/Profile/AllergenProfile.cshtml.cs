using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace WebApp.Pages.Profile;

public class AllergenProfileModel : PageModel
{
    private readonly IAllergenService _allergenService;
    private readonly IUserProfileService _userProfileService;
    private const string AllergenProfileCookieName = "AllergenProfile";

    public AllergenProfileModel(
        IAllergenService allergenService,
        IUserProfileService userProfileService)
    {
        _allergenService = allergenService;
        _userProfileService = userProfileService;
    }

    [BindProperty]
    public List<int> ExcludedAllergenIds { get; set; } = new();

    public List<SelectListItem> AvailableAllergens { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            await LoadAllergenProfileAsync();
            await LoadAllergensAsync();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load allergen profile. Please try again.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await SaveAllergenProfileAsync();
            TempData["SuccessMessage"] = "Allergen profile saved successfully! Recipes containing these allergens will be automatically hidden.";
            await LoadAllergensAsync();
            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to save allergen profile. Please try again.";
            await LoadAllergensAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostClearAsync()
    {
        try
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _userProfileService.UpdateAllergenProfileAsync(userId, new List<int>());
            }
            else
            {
                Response.Cookies.Delete(AllergenProfileCookieName);
            }

            ExcludedAllergenIds.Clear();
            TempData["SuccessMessage"] = "Allergen profile cleared successfully!";
            return RedirectToPage();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to clear allergen profile. Please try again.";
            return RedirectToPage();
        }
    }

    private async Task LoadAllergenProfileAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ExcludedAllergenIds = await _userProfileService.GetUserAllergenProfileAsync(userId);
        }
        else
        {
            // Fall back to cookies for non-authenticated users
            if (Request.Cookies.TryGetValue(AllergenProfileCookieName, out var cookieValue))
            {
                var allergenIds = cookieValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
                ExcludedAllergenIds = allergenIds;
            }
        }
    }

    private async Task SaveAllergenProfileAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _userProfileService.UpdateAllergenProfileAsync(userId, ExcludedAllergenIds);
        }
        else
        {
            // Fall back to cookies for non-authenticated users
            var cookieValue = string.Join(",", ExcludedAllergenIds);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append(AllergenProfileCookieName, cookieValue, cookieOptions);
        }
    }

    private async Task LoadAllergensAsync()
    {
        var allergens = await _allergenService.GetAllAsync();
        AvailableAllergens = allergens
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name,
                Selected = ExcludedAllergenIds.Contains(a.Id)
            })
            .ToList();
    }
}
