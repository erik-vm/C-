using BLL.DTOs;

namespace BLL.Services;

public interface IRecipeService
{
    Task<RecipeDto?> GetByIdAsync(int id);
    Task<RecipeDetailDto?> GetDetailByIdAsync(int id);
    Task<IEnumerable<RecipeDto>> GetAllAsync();
    Task<PagedResultDto<RecipeDto>> SearchAsync(RecipeSearchDto searchDto);
    Task<IEnumerable<WhatCanIMakeDto>> WhatCanIMakeAsync(List<int> availableIngredientIds, int minMatchPercentage = 70);

    Task<RecipeDto> CreateAsync(CreateRecipeDto dto);
    Task<RecipeDto> UpdateAsync(int id, UpdateRecipeDto dto);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
    Task<IEnumerable<RecipeDto>> GetDeletedAsync();

    Task<ScaledRecipeDto> GetScaledRecipeAsync(int id, int servings);
    Task<IEnumerable<string>> ValidateRecipeAsync(CreateRecipeDto dto);
}
