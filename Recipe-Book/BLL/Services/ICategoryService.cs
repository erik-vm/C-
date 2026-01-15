using BLL.DTOs;

namespace BLL.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
}
