using BLL.DTOs;
using BLL.Exceptions;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class RecipeService : IRecipeService
{
    private readonly RecipeDbContext _context;
    private readonly IScalingService _scalingService;

    public RecipeService(RecipeDbContext context, IScalingService scalingService)
    {
        _context = context;
        _scalingService = scalingService;
    }

    public async Task<RecipeDto?> GetByIdAsync(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.IngredientAllergens)
                        .ThenInclude(ia => ia.Allergen)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return null;

        return MapToRecipeDto(recipe);
    }

    public async Task<RecipeDetailDto?> GetDetailByIdAsync(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.IngredientAllergens)
                        .ThenInclude(ia => ia.Allergen)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return null;

        return new RecipeDetailDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            DescriptionHtml = ConvertMarkdownToHtml(recipe.Description),
            BaseServingSize = recipe.BaseServingSize,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            DifficultyLevel = recipe.DifficultyLevel.ToString(),
            Categories = recipe.RecipeCategories.Select(rc => rc.Category.Name).ToList(),
            Ingredients = recipe.RecipeIngredients
                .OrderBy(ri => ri.SortOrder)
                .Select(ri => new RecipeIngredientDto
                {
                    Id = ri.Id,
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient.Name,
                    Amount = ri.Amount,
                    Unit = ri.Unit.ToString(),
                    DisplayAmount = FormatAmount(ri.Amount, ri.Unit),
                    IsOptional = ri.IsOptional,
                    PrepNote = ri.PrepNote,
                    Allergens = ri.Ingredient.IngredientAllergens
                        .Select(ia => ia.Allergen.Name)
                        .ToList()
                }).ToList(),
            CreatedAt = recipe.CreatedAt,
            ModifiedAt = recipe.ModifiedAt
        };
    }

    public async Task<IEnumerable<RecipeDto>> GetAllAsync()
    {
        var recipes = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.IngredientAllergens)
                        .ThenInclude(ia => ia.Allergen)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .ToListAsync();

        return recipes.Select(MapToRecipeDto);
    }

    public async Task<PagedResultDto<RecipeDto>> SearchAsync(RecipeSearchDto searchDto)
    {
        var query = _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.IngredientAllergens)
                        .ThenInclude(ia => ia.Allergen)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            var searchLower = searchDto.SearchTerm.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(searchLower) ||
                r.Description.ToLower().Contains(searchLower) ||
                r.RecipeIngredients.Any(ri => ri.Ingredient.Name.ToLower().Contains(searchLower))
            );
        }

        if (searchDto.CategoryIds.Any())
        {
            query = query.Where(r => r.RecipeCategories.Any(rc => searchDto.CategoryIds.Contains(rc.CategoryId)));
        }

        if (searchDto.DifficultyLevels.Any())
        {
            query = query.Where(r => searchDto.DifficultyLevels.Contains((int)r.DifficultyLevel));
        }

        if (searchDto.MaxTotalTimeMinutes.HasValue)
        {
            query = query.Where(r => (r.PrepTimeMinutes + r.CookTimeMinutes) <= searchDto.MaxTotalTimeMinutes.Value);
        }

        if (searchDto.MustContainIngredientIds.Any())
        {
            foreach (var ingredientId in searchDto.MustContainIngredientIds)
            {
                query = query.Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == ingredientId));
            }
        }

        if (searchDto.MustNotContainIngredientIds.Any())
        {
            query = query.Where(r => !r.RecipeIngredients.Any(ri => searchDto.MustNotContainIngredientIds.Contains(ri.IngredientId)));
        }

        if (searchDto.ExcludeAllergenIds.Any())
        {
            query = query.Where(r => !r.RecipeIngredients.Any(ri =>
                ri.Ingredient.IngredientAllergens.Any(ia => searchDto.ExcludeAllergenIds.Contains(ia.AllergenId))
            ));
        }

        if (searchDto.VegetarianOnly == true)
        {
            query = query.Where(r => r.RecipeIngredients.Where(ri => !ri.IsOptional).All(ri => ri.Ingredient.IsVegetarian));
        }

        if (searchDto.VeganOnly == true)
        {
            query = query.Where(r => r.RecipeIngredients.Where(ri => !ri.IsOptional).All(ri => ri.Ingredient.IsVegan));
        }

        var totalCount = await query.CountAsync();

        var recipes = await query
            .OrderBy(r => r.Name)
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return new PagedResultDto<RecipeDto>
        {
            Items = recipes.Select(MapToRecipeDto).ToList(),
            TotalCount = totalCount,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize
        };
    }

    public async Task<IEnumerable<WhatCanIMakeDto>> WhatCanIMakeAsync(List<int> availableIngredientIds, int minMatchPercentage = 70)
    {
        var allRecipes = await _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.IngredientAllergens)
                        .ThenInclude(ia => ia.Allergen)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .ToListAsync();

        var results = new List<WhatCanIMakeDto>();

        foreach (var recipe in allRecipes)
        {
            var requiredIngredients = recipe.RecipeIngredients
                .Where(ri => !ri.IsOptional)
                .ToList();

            if (!requiredIngredients.Any())
                continue;

            var requiredIngredientIds = requiredIngredients.Select(ri => ri.IngredientId).ToList();
            var matchedIngredientIds = requiredIngredientIds.Intersect(availableIngredientIds).ToList();
            var matchPercentage = (int)Math.Round((decimal)matchedIngredientIds.Count / requiredIngredientIds.Count * 100);

            if (matchPercentage >= minMatchPercentage)
            {
                var missingIngredientIds = requiredIngredientIds.Except(matchedIngredientIds).ToList();
                var missingIngredients = requiredIngredients
                    .Where(ri => missingIngredientIds.Contains(ri.IngredientId))
                    .Select(ri => ri.Ingredient.Name)
                    .ToList();

                var matchedIngredients = requiredIngredients
                    .Where(ri => matchedIngredientIds.Contains(ri.IngredientId))
                    .Select(ri => ri.Ingredient.Name)
                    .ToList();

                var allergens = recipe.RecipeIngredients
                    .SelectMany(ri => ri.Ingredient.IngredientAllergens)
                    .Select(ia => ia.Allergen.Name)
                    .Distinct()
                    .ToList();

                results.Add(new WhatCanIMakeDto
                {
                    RecipeId = recipe.Id,
                    RecipeName = recipe.Name,
                    Description = recipe.Description,
                    BaseServingSize = recipe.BaseServingSize,
                    TotalTimeMinutes = recipe.PrepTimeMinutes + recipe.CookTimeMinutes,
                    DifficultyLevel = recipe.DifficultyLevel.ToString(),
                    Categories = recipe.RecipeCategories.Select(rc => rc.Category.Name).ToList(),
                    MatchPercentage = matchPercentage,
                    RequiredIngredientsCount = requiredIngredientIds.Count,
                    MatchedIngredientsCount = matchedIngredientIds.Count,
                    MissingIngredients = missingIngredients,
                    MatchedIngredients = matchedIngredients,
                    HasAllergens = allergens.Any(),
                    AllergenWarnings = allergens
                });
            }
        }

        return results.OrderByDescending(r => r.MatchPercentage).ThenBy(r => r.MissingIngredients.Count);
    }

    public async Task<ScaledRecipeDto> GetScaledRecipeAsync(int id, int servings)
    {
        var recipe = await GetDetailByIdAsync(id);
        if (recipe == null)
            throw new BusinessException($"Recipe with ID {id} not found");

        var scaledIngredients = recipe.Ingredients
            .Select(ing => _scalingService.ScaleIngredient(
                ing,
                recipe.BaseServingSize,
                servings))
            .ToList();

        return new ScaledRecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            OriginalServings = recipe.BaseServingSize,
            ScaledServings = servings,
            ScaleFactor = (decimal)servings / recipe.BaseServingSize,
            Ingredients = scaledIngredients,
            HasScalingWarning = _scalingService.ShouldWarnAboutScaling(
                recipe.BaseServingSize, servings),
            ScalingWarningMessage = _scalingService.GetScalingWarningMessage(
                recipe.BaseServingSize, servings)
        };
    }

    public async Task<RecipeDto> CreateAsync(CreateRecipeDto dto)
    {
        var validationErrors = await ValidateRecipeAsync(dto);
        if (validationErrors.Any())
            throw new ValidationException(validationErrors.ToList());

        var exists = await _context.Recipes
            .AnyAsync(r => r.Name.ToLower() == dto.Name.ToLower() && !r.IsDeleted);
        if (exists)
            throw new BusinessException($"Recipe with name '{dto.Name}' already exists");

        var recipe = new Recipe
        {
            Name = dto.Name,
            Description = dto.Description,
            BaseServingSize = dto.BaseServingSize,
            PrepTimeMinutes = dto.PrepTimeMinutes,
            CookTimeMinutes = dto.CookTimeMinutes,
            DifficultyLevel = (DifficultyLevel)dto.DifficultyLevel,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        foreach (var categoryId in dto.CategoryIds)
        {
            recipe.RecipeCategories.Add(new RecipeCategory { CategoryId = categoryId });
        }

        foreach (var ingredientDto in dto.Ingredients)
        {
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = ingredientDto.IngredientId,
                Amount = ingredientDto.Amount,
                Unit = (MeasurementUnit)ingredientDto.Unit,
                IsOptional = ingredientDto.IsOptional,
                PrepNote = ingredientDto.PrepNote,
                SortOrder = ingredientDto.SortOrder
            });
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(recipe.Id)
            ?? throw new BusinessException("Failed to retrieve created recipe");
    }

    public async Task<RecipeDto> UpdateAsync(int id, UpdateRecipeDto dto)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeCategories)
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (recipe == null)
            throw new BusinessException($"Recipe with ID {id} not found");

        var nameExists = await _context.Recipes
            .AnyAsync(r => r.Id != id && r.Name.ToLower() == dto.Name.ToLower() && !r.IsDeleted);
        if (nameExists)
            throw new BusinessException($"Recipe with name '{dto.Name}' already exists");

        recipe.Name = dto.Name;
        recipe.Description = dto.Description;
        recipe.BaseServingSize = dto.BaseServingSize;
        recipe.PrepTimeMinutes = dto.PrepTimeMinutes;
        recipe.CookTimeMinutes = dto.CookTimeMinutes;
        recipe.DifficultyLevel = (DifficultyLevel)dto.DifficultyLevel;
        recipe.ModifiedAt = DateTime.UtcNow;

        _context.RecipeCategories.RemoveRange(recipe.RecipeCategories);
        foreach (var categoryId in dto.CategoryIds)
        {
            recipe.RecipeCategories.Add(new RecipeCategory { CategoryId = categoryId });
        }

        _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
        foreach (var ingredientDto in dto.Ingredients)
        {
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = ingredientDto.IngredientId,
                Amount = ingredientDto.Amount,
                Unit = (MeasurementUnit)ingredientDto.Unit,
                IsOptional = ingredientDto.IsOptional,
                PrepNote = ingredientDto.PrepNote,
                SortOrder = ingredientDto.SortOrder
            });
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(recipe.Id)
            ?? throw new BusinessException("Failed to retrieve updated recipe");
    }

    public async Task DeleteAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            throw new BusinessException($"Recipe with ID {id} not found");

        recipe.IsDeleted = true;
        recipe.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var recipe = await _context.Recipes.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id);
        if (recipe == null)
            throw new BusinessException($"Recipe with ID {id} not found");

        recipe.IsDeleted = false;
        recipe.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RecipeDto>> GetDeletedAsync()
    {
        var recipes = await _context.Recipes
            .IgnoreQueryFilters()
            .Where(r => r.IsDeleted)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.IngredientAllergens)
                        .ThenInclude(ia => ia.Allergen)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return recipes.Select(MapToRecipeDto);
    }

    public async Task<IEnumerable<string>> ValidateRecipeAsync(CreateRecipeDto dto)
    {
        var errors = new List<string>();

        var hasRequiredIngredient = dto.Ingredients.Any(i => !i.IsOptional);
        if (!hasRequiredIngredient)
            errors.Add("Recipe must have at least one required (non-optional) ingredient");

        var duplicateIngredients = dto.Ingredients
            .GroupBy(i => i.IngredientId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        if (duplicateIngredients.Any())
            errors.Add("Recipe cannot contain the same ingredient multiple times");

        var categoryIds = dto.CategoryIds.Distinct().ToList();
        var validCategoryIds = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync();

        if (validCategoryIds.Count != categoryIds.Count)
            errors.Add("One or more category IDs are invalid");

        var ingredientIds = dto.Ingredients.Select(i => i.IngredientId).Distinct().ToList();
        var validIngredientIds = await _context.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .Select(i => i.Id)
            .ToListAsync();

        if (validIngredientIds.Count != ingredientIds.Count)
            errors.Add("One or more ingredient IDs are invalid");

        return errors;
    }

    private RecipeDto MapToRecipeDto(Recipe recipe)
    {
        var allergens = recipe.RecipeIngredients
            .SelectMany(ri => ri.Ingredient.IngredientAllergens)
            .Select(ia => ia.Allergen.Name)
            .Distinct()
            .ToList();

        return new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            BaseServingSize = recipe.BaseServingSize,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            DifficultyLevel = recipe.DifficultyLevel.ToString(),
            Categories = recipe.RecipeCategories.Select(rc => rc.Category.Name).ToList(),
            HasAllergens = allergens.Any(),
            AllergenWarnings = allergens
        };
    }

    private string ConvertMarkdownToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var html = markdown;

        // Handle line breaks - replace double newlines with paragraph breaks
        html = html.Replace("\r\n\r\n", "</p><p>");
        html = html.Replace("\n\n", "</p><p>");
        html = html.Replace("\r\n", "<br/>");
        html = html.Replace("\n", "<br/>");

        // Wrap in paragraph tags if not already
        if (!html.StartsWith("<p>"))
        {
            html = $"<p>{html}</p>";
        }

        return html;
    }

    private string FormatAmount(decimal amount, MeasurementUnit unit)
    {
        return $"{amount:0.##} {unit}";
    }
}
