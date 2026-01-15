namespace DAL.Entities;

public class UserAllergenProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int AllergenId { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;
    public Allergen Allergen { get; set; } = null!;
}
