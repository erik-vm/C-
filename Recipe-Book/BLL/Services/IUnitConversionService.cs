using BLL.DTOs;
using DAL.Entities;

namespace BLL.Services;

public interface IUnitConversionService
{
    string ConvertToImperial(decimal amount, MeasurementUnit unit);
    bool IsMetricUnit(MeasurementUnit unit);
}
