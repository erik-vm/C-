using DAL.Entities;

namespace BLL.Services;

public class UnitConversionService : IUnitConversionService
{
    public string ConvertToImperial(decimal amount, MeasurementUnit unit)
    {
        return unit switch
        {
            // Weight conversions
            MeasurementUnit.Grams => ConvertGramsToOunces(amount),
            MeasurementUnit.Kilograms => ConvertKilogramsToPounds(amount),

            // Volume conversions
            MeasurementUnit.Milliliters => ConvertMillilitersToFluidOunces(amount),
            MeasurementUnit.Liters => ConvertLitersToQuarts(amount),

            // Already imperial or universal units - no conversion
            _ => $"{FormatAmount(amount)} {unit}"
        };
    }

    public bool IsMetricUnit(MeasurementUnit unit)
    {
        return unit is MeasurementUnit.Grams
            or MeasurementUnit.Kilograms
            or MeasurementUnit.Milliliters
            or MeasurementUnit.Liters;
    }

    private string ConvertGramsToOunces(decimal grams)
    {
        var ounces = grams * 0.035274m;

        if (ounces >= 16)
        {
            var pounds = ounces / 16;
            return $"{FormatAmount(pounds)} lb";
        }

        return $"{FormatAmount(ounces)} oz";
    }

    private string ConvertKilogramsToPounds(decimal kilograms)
    {
        var pounds = kilograms * 2.20462m;
        return $"{FormatAmount(pounds)} lb";
    }

    private string ConvertMillilitersToFluidOunces(decimal milliliters)
    {
        var flOz = milliliters * 0.033814m;

        if (flOz >= 128)
        {
            var gallons = flOz / 128;
            return $"{FormatAmount(gallons)} gal";
        }
        else if (flOz >= 32)
        {
            var quarts = flOz / 32;
            return $"{FormatAmount(quarts)} qt";
        }
        else if (flOz >= 16)
        {
            var pints = flOz / 16;
            return $"{FormatAmount(pints)} pt";
        }
        else if (flOz >= 8)
        {
            var cups = flOz / 8;
            return $"{FormatAmount(cups)} cup";
        }

        return $"{FormatAmount(flOz)} fl oz";
    }

    private string ConvertLitersToQuarts(decimal liters)
    {
        var quarts = liters * 1.05669m;

        if (quarts >= 4)
        {
            var gallons = quarts / 4;
            return $"{FormatAmount(gallons)} gal";
        }

        return $"{FormatAmount(quarts)} qt";
    }

    private string FormatAmount(decimal amount)
    {
        if (amount >= 10)
            return Math.Round(amount, 0).ToString("0");
        else if (amount >= 1)
            return amount.ToString("0.##");
        else
            return amount.ToString("0.##");
    }
}
