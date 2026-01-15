using BLL.DTOs;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class AllergenService : IAllergenService
{
    private readonly RecipeDbContext _context;

    public AllergenService(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AllergenDto>> GetAllAsync()
    {
        var allergens = await _context.Allergens
            .OrderBy(a => a.Name)
            .ToListAsync();

        return allergens.Select(a => new AllergenDto
        {
            Id = a.Id,
            Name = a.Name
        });
    }
}
