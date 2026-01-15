using BLL.DTOs;
using BLL.Exceptions;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class UserProfileService : IUserProfileService
{
    private readonly RecipeDbContext _context;

    public UserProfileService(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.UserAllergenProfiles)
                .ThenInclude(uap => uap.Allergen)
            .Include(u => u.FavoriteRecipes)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        return new UserProfileDto
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            AllergenProfileIds = user.UserAllergenProfiles.Select(uap => uap.AllergenId).ToList(),
            AllergenNames = user.UserAllergenProfiles.Select(uap => uap.Allergen.Name).ToList(),
            FavoriteRecipesCount = user.FavoriteRecipes.Count,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task UpdateAllergenProfileAsync(string userId, List<int> allergenIds)
    {
        // Remove existing allergen profiles
        var existing = await _context.UserAllergenProfiles
            .Where(uap => uap.UserId == userId)
            .ToListAsync();

        _context.UserAllergenProfiles.RemoveRange(existing);

        // Add new allergen profiles
        foreach (var allergenId in allergenIds)
        {
            _context.UserAllergenProfiles.Add(new UserAllergenProfile
            {
                UserId = userId,
                AllergenId = allergenId
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetUserAllergenProfileAsync(string userId)
    {
        return await _context.UserAllergenProfiles
            .Where(uap => uap.UserId == userId)
            .Select(uap => uap.AllergenId)
            .ToListAsync();
    }

    public async Task AddFavoriteRecipeAsync(string userId, int recipeId)
    {
        try
        {
            // Check if recipe exists (respect soft delete filter)
            var recipeExists = await _context.Recipes
                .Where(r => r.Id == recipeId)
                .AnyAsync();
            if (!recipeExists)
                throw new BusinessException($"Recipe with ID {recipeId} not found");

            // Check if already favorited
            var exists = await _context.FavoriteRecipes
                .AnyAsync(fr => fr.UserId == userId && fr.RecipeId == recipeId);

            if (exists)
                return; // Already favorited, no action needed

            var favorite = new FavoriteRecipe
            {
                UserId = userId,
                RecipeId = recipeId,
                AddedAt = DateTime.UtcNow
            };

            _context.FavoriteRecipes.Add(favorite);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new BusinessException($"Failed to add favorite: {ex.Message}", ex);
        }
    }

    public async Task RemoveFavoriteRecipeAsync(string userId, int recipeId)
    {
        var favorite = await _context.FavoriteRecipes
            .FirstOrDefaultAsync(fr => fr.UserId == userId && fr.RecipeId == recipeId);

        if (favorite != null)
        {
            _context.FavoriteRecipes.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsFavoriteAsync(string userId, int recipeId)
    {
        return await _context.FavoriteRecipes
            .AnyAsync(fr => fr.UserId == userId && fr.RecipeId == recipeId);
    }

    public async Task<List<RecipeDto>> GetFavoriteRecipesAsync(string userId)
    {
        var favoriteRecipes = await _context.FavoriteRecipes
            .Where(fr => fr.UserId == userId && fr.Recipe != null)
            .Include(fr => fr.Recipe!)
                .ThenInclude(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                        .ThenInclude(i => i.IngredientAllergens)
                            .ThenInclude(ia => ia.Allergen)
            .Include(fr => fr.Recipe!)
                .ThenInclude(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
            .OrderByDescending(fr => fr.AddedAt)
            .ToListAsync();

        var recipeDtos = new List<RecipeDto>();

        foreach (var favoriteRecipe in favoriteRecipes)
        {
            var recipe = favoriteRecipe.Recipe;
            if (recipe == null) continue; // Skip if recipe was soft-deleted

            var allergens = recipe.RecipeIngredients
                .Where(ri => !ri.IsOptional)
                .SelectMany(ri => ri.Ingredient.IngredientAllergens)
                .Select(ia => ia.Allergen.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            recipeDtos.Add(new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                BaseServingSize = recipe.BaseServingSize,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                DifficultyLevel = recipe.DifficultyLevel.ToString(),
                Categories = recipe.RecipeCategories.Select(rc => rc.Category.Name).ToList(),
                AllergenWarnings = allergens,
                HasAllergens = allergens.Any()
            });
        }

        return recipeDtos;
    }
}
