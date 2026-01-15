using BLL.DTOs;

namespace BLL.Services;

public interface IIngredientService
{
    Task<IngredientDto?> GetByIdAsync(int id);
    Task<IEnumerable<IngredientDto>> GetAllAsync();
    Task<IngredientDto> CreateAsync(CreateIngredientDto dto);
}
