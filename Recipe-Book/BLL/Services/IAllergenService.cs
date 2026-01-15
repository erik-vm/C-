using BLL.DTOs;

namespace BLL.Services;

public interface IAllergenService
{
    Task<IEnumerable<AllergenDto>> GetAllAsync();
}
