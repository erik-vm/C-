using BLL.DTOs;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly RecipeDbContext _context;

    public CategoryService(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
    }
}
