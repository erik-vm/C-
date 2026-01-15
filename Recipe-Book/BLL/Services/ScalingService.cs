using BLL.DTOs;
using DAL.Entities;

namespace BLL.Services;

public class ScalingService : IScalingService
{
    public ScaledIngredientDto ScaleIngredient(
        RecipeIngredientDto ingredient,
        int originalServings,
        int targetServings)
    {
        var scaleFactor = (decimal)targetServings / originalServings;
        var rawScaledAmount = ingredient.Amount * scaleFactor;

        var unit = Enum.Parse<MeasurementUnit>(ingredient.Unit);
        var (scaledAmount, scaledUnit) = SmartRound(rawScaledAmount, unit);

        return new ScaledIngredientDto
        {
            Name = ingredient.IngredientName,
            OriginalAmount = ingredient.Amount,
            OriginalUnit = ingredient.Unit,
            ScaledAmount = scaledAmount,
            ScaledUnit = scaledUnit.ToString(),
            DisplayAmount = FormatAmount(scaledAmount, scaledUnit),
            IsOptional = ingredient.IsOptional,
            PrepNote = ingredient.PrepNote
        };
    }

    private (decimal amount, MeasurementUnit unit) SmartRound(
        decimal rawAmount,
        MeasurementUnit originalUnit)
    {
        if (originalUnit == MeasurementUnit.Pinch || originalUnit == MeasurementUnit.ToTaste)
            return (1, originalUnit);

        if (originalUnit == MeasurementUnit.Whole || originalUnit == MeasurementUnit.Pieces)
        {
            return (Math.Max(1, Math.Round(rawAmount)), originalUnit);
        }

        if (originalUnit == MeasurementUnit.Teaspoons ||
            originalUnit == MeasurementUnit.Tablespoons)
        {
            var rounded = Math.Round(rawAmount * 4) / 4;

            if (originalUnit == MeasurementUnit.Teaspoons && rounded >= 3)
            {
                var tablespoons = rounded / 3;
                return SmartRound(tablespoons, MeasurementUnit.Tablespoons);
            }

            if (originalUnit == MeasurementUnit.Tablespoons && rounded >= 16)
            {
                var cups = rounded / 16;
                return SmartRound(cups, MeasurementUnit.Cups);
            }

            return (rounded, originalUnit);
        }

        if (originalUnit == MeasurementUnit.Grams ||
            originalUnit == MeasurementUnit.Milliliters)
        {
            var rounded = Math.Round(rawAmount);

            if (rounded >= 1000)
            {
                var larger = rounded / 1000;
                var largerUnit = originalUnit == MeasurementUnit.Grams
                    ? MeasurementUnit.Kilograms
                    : MeasurementUnit.Liters;
                return (Math.Round(larger, 2), largerUnit);
            }

            if (rounded > 100)
                rounded = Math.Round(rounded / 5) * 5;

            return (rounded, originalUnit);
        }

        if (originalUnit == MeasurementUnit.Cups)
        {
            var rounded = Math.Round(rawAmount * 4) / 4;
            return (rounded, originalUnit);
        }

        return (Math.Round(rawAmount, 2), originalUnit);
    }

    public string FormatAmount(decimal amount, MeasurementUnit unit)
    {
        if (unit == MeasurementUnit.Pinch)
            return "a pinch";

        if (unit == MeasurementUnit.ToTaste)
            return "to taste";

        if (unit == MeasurementUnit.Cups ||
            unit == MeasurementUnit.Teaspoons ||
            unit == MeasurementUnit.Tablespoons)
        {
            var fractionStr = DecimalToFraction(amount);
            return $"{fractionStr} {GetUnitAbbreviation(unit)}";
        }

        return $"{amount:0.##} {GetUnitAbbreviation(unit)}";
    }

    public bool ShouldWarnAboutScaling(int originalServings, int targetServings)
    {
        var factor = (decimal)targetServings / originalServings;

        if (factor > 10) return true;

        if (factor < 0.25m) return true;

        return false;
    }

    public string? GetScalingWarningMessage(int originalServings, int targetServings)
    {
        var factor = (decimal)targetServings / originalServings;

        if (factor > 10)
            return "Warning: Scaling this recipe by more than 10x may produce impractical measurements. " +
                   "Consider making multiple batches instead.";

        if (factor < 0.25m)
            return "Warning: Scaling this recipe below quarter size may lose precision in measurements.";

        return null;
    }

    private string DecimalToFraction(decimal value)
    {
        var whole = (int)Math.Floor(value);
        var fraction = value - whole;

        if (fraction == 0)
            return whole.ToString();

        if (fraction == 0.25m)
            return whole > 0 ? $"{whole} 1/4" : "1/4";
        if (fraction == 0.5m)
            return whole > 0 ? $"{whole} 1/2" : "1/2";
        if (fraction == 0.75m)
            return whole > 0 ? $"{whole} 3/4" : "3/4";

        return value.ToString("0.##");
    }

    private string GetUnitAbbreviation(MeasurementUnit unit)
    {
        return unit switch
        {
            MeasurementUnit.Teaspoons => "tsp",
            MeasurementUnit.Tablespoons => "tbsp",
            MeasurementUnit.Cups => "cup",
            MeasurementUnit.Grams => "g",
            MeasurementUnit.Kilograms => "kg",
            MeasurementUnit.Milliliters => "ml",
            MeasurementUnit.Liters => "L",
            MeasurementUnit.Ounces => "oz",
            MeasurementUnit.Pounds => "lb",
            MeasurementUnit.FluidOunces => "fl oz",
            MeasurementUnit.Pints => "pt",
            MeasurementUnit.Quarts => "qt",
            MeasurementUnit.Pieces => "pieces",
            MeasurementUnit.Whole => "",
            _ => unit.ToString().ToLower()
        };
    }
}
