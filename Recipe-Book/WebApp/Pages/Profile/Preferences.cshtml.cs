using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Profile;

public class PreferencesModel : PageModel
{
    private const string UnitSystemCookieName = "UnitSystem";

    [BindProperty]
    public string UnitSystem { get; set; } = "Metric";

    public void OnGet()
    {
        try
        {
            LoadUnitSystemFromCookie();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load preferences. Using default settings.";
        }
    }

    public IActionResult OnPost()
    {
        try
        {
            SaveUnitSystemToCookie();
            TempData["SuccessMessage"] = $"Preferences saved! Unit system set to {UnitSystem}.";
            return Page();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to save preferences. Please try again.";
            return Page();
        }
    }

    private void LoadUnitSystemFromCookie()
    {
        if (Request.Cookies.TryGetValue(UnitSystemCookieName, out var cookieValue))
        {
            UnitSystem = cookieValue;
        }
    }

    private void SaveUnitSystemToCookie()
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddYears(1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append(UnitSystemCookieName, UnitSystem, cookieOptions);
    }
}
