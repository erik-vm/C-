using BLL.DTOs;
using DAL.Entities;

namespace BLL.Services;

public interface IScalingService
{
    ScaledIngredientDto ScaleIngredient(
        RecipeIngredientDto ingredient,
        int originalServings,
        int targetServings);

    string FormatAmount(decimal amount, MeasurementUnit unit);

    bool ShouldWarnAboutScaling(int originalServings, int targetServings);

    string? GetScalingWarningMessage(int originalServings, int targetServings);
}
