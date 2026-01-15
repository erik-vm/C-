using BLL.DTOs;
using BLL.Exceptions;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class IngredientService : IIngredientService
{
    private readonly RecipeDbContext _context;

    public IngredientService(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<IngredientDto?> GetByIdAsync(int id)
    {
        var ingredient = await _context.Ingredients
            .Include(i => i.IngredientAllergens)
                .ThenInclude(ia => ia.Allergen)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingredient == null) return null;

        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            DefaultUnit = ingredient.DefaultUnit.ToString(),
            Category = ingredient.Category.ToString(),
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            Allergens = ingredient.IngredientAllergens
                .Select(ia => ia.Allergen.Name)
                .ToList()
        };
    }

    public async Task<IEnumerable<IngredientDto>> GetAllAsync()
    {
        var ingredients = await _context.Ingredients
            .Include(i => i.IngredientAllergens)
                .ThenInclude(ia => ia.Allergen)
            .OrderBy(i => i.Name)
            .ToListAsync();

        return ingredients.Select(ingredient => new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            DefaultUnit = ingredient.DefaultUnit.ToString(),
            Category = ingredient.Category.ToString(),
            IsVegetarian = ingredient.IsVegetarian,
            IsVegan = ingredient.IsVegan,
            Allergens = ingredient.IngredientAllergens
                .Select(ia => ia.Allergen.Name)
                .ToList()
        });
    }

    public async Task<IngredientDto> CreateAsync(CreateIngredientDto dto)
    {
        var exists = await _context.Ingredients
            .AnyAsync(i => i.Name.ToLower() == dto.Name.ToLower());
        if (exists)
            throw new BusinessException($"Ingredient with name '{dto.Name}' already exists");

        if (dto.IsVegan && !dto.IsVegetarian)
            throw new BusinessException("Vegan ingredients must also be vegetarian");

        var ingredient = new Ingredient
        {
            Name = dto.Name,
            DefaultUnit = (MeasurementUnit)dto.DefaultUnit,
            Category = (IngredientCategory)dto.Category,
            IsVegetarian = dto.IsVegetarian,
            IsVegan = dto.IsVegan
        };

        foreach (var allergenId in dto.AllergenIds)
        {
            ingredient.IngredientAllergens.Add(new IngredientAllergen
            {
                AllergenId = allergenId
            });
        }

        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(ingredient.Id)
            ?? throw new BusinessException("Failed to retrieve created ingredient");
    }
}
