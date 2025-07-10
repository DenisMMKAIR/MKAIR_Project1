using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.ProtocolCalculations;

internal static class Manometr1Calculations
{
    public static Manometr1Verification ToManometr1(ILogger logger, SuccessVerification v, VerificationMethod[] vrfMeth)
    {
        var vms = vrfMeth.First(m => m.Aliases.Contains(v.VerificationTypeName));

        var newVrf = new Manometr1Verification
        {
            VerificationGroup = v.VerificationGroup,
            Address = MKAIRInfo.GetAddress(v.VerificationDate),
            ProtocolNumber = v.ProtocolNumber,
            DeviceTypeName = v.DeviceTypeNumber,
            DeviceModification = v.Device!.Modification,
            DeviceTypeNumber = v.DeviceTypeNumber,
            DeviceSerial = v.DeviceSerial,
            ManufactureYear = v.Device!.ManufacturedYear,
            Owner = v.Owner,
            OwnerINN = v.OwnerINN,
            VerificationsInfo = v.VerificationTypeName,
            EtalonsInfo = v.Etalons!.Select(e => e.FullInfo).Aggregate((a, c) => $"{a}; {c}"),
            Temperature = v.Temperature,
            Humidity = v.Humidity,
            Pressure = v.Pressure,
            VerificationVisualCheckup = vms.Checkups["visual"],
            VerificationResultCheckup = vms.Checkups["result"],
            VerificationAccuracyCheckup = vms.Checkups["accuracy"],
            VerificationDate = v.VerificationDate,
            Worker = v.Worker,

            // Support properties
            Location = v.Location,
            VerifiedUntilDate = v.VerifiedUntilDate,

            // Table values
            MeasurementMin = (double)v.AdditionalInfo["min"],
            MeasurementMax = (double)v.AdditionalInfo["max"],
            MeasurementUnit = (string)v.AdditionalInfo["unit"],
            ValidError = (double)v.AdditionalInfo["validError"],

            DeviceValues = default!,
            EtalonValues = default!,
            ActualError = default!,
            ActualVariation = default!,

            // Navigation properties
            Device = v.Device,
            Etalons = v.Etalons,
            VerificationMethod = vms
        };

        CalculateValues(logger, newVrf);

        return newVrf;
    }

    private static void CalculateValues(ILogger logger, Manometr1Verification newVrf)
    {
        var magicValues = new[] { 4, 8, 12, 16, 20, 24, 28, 32 };

        var min = newVrf.MeasurementMin;
        var max = newVrf.MeasurementMax;
        var error = newVrf.ValidError;
        var errorValue = (max - min) / 100 * error;

        double[][] etalonValues = [[0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0, 0, 0]];
        double[][] deviceValues = [[0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0, 0, 0]];
        double[][] actualErrors = [[0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0, 0, 0]];
        double[] actualVariations = [double.NaN, 0, 0, 0, 0, 0, 0, double.NaN];

        for (var i = 0; i < magicValues.Length; i++)
        {
            var forwardEtalon = etalonValues[0][i] = (max - min) / 100 * ((magicValues[i] - 4) / 28 * 100) + min;

            var minRange = Math.Max(min, forwardEtalon - errorValue * 0.7);
            var maxRange = Math.Min(max, forwardEtalon + errorValue * 0.7);

            var forwardValue = deviceValues[0][i] = minRange + (maxRange - minRange) * Random.Shared.NextDouble();

            var forwardActualError = actualErrors[0][i] = (forwardValue - forwardEtalon) / (max - min) * 100;

            if (Math.Abs(forwardActualError) > error)
            {
                var msg = $"У {newVrf} ошибка прямого хода {forwardActualError:N3} превышает допустимую {error:N3}";
                logger.LogError("{Msg}", msg);
                throw new Exception(msg);
            }

            var backwardEtalon = etalonValues[1][i] = (max - min) / 100 * ((magicValues[i] - 4) / 28 * 100) + min;

            minRange = Math.Max(min, backwardEtalon - errorValue * 0.7);
            minRange = Math.Max(minRange, forwardValue - errorValue * 0.48);
            maxRange = Math.Min(max, backwardEtalon + errorValue * 0.7);
            maxRange = Math.Min(maxRange, forwardValue + errorValue * 0.48);

            var backwardValue = deviceValues[1][i] = minRange + (maxRange - minRange) * Random.Shared.NextDouble();

            var backwardActualError = actualErrors[1][i] = (backwardValue - backwardEtalon) / (max - min) * 100;

            if (Math.Abs(backwardActualError) > error)
            {
                var msg = $"У {newVrf} ошибка обратного хода {backwardActualError:N3} превышает допустимую {error:N3}";
                logger.LogError("{Msg}", msg);
                throw new Exception(msg);
            }

            if (i == 0 || i == magicValues.Length - 1) continue;

            var actualVariation = actualVariations[i] = (forwardValue - backwardValue) / (max - min) * 100;

            if (Math.Abs(actualVariation) > error)
            {
                var msg = $"У {newVrf} ошибка вариации {actualVariation:N3} превышает допустимую {error:N3}";
                logger.LogError("{Msg}", msg);
                throw new Exception(msg);
            }
        }

        newVrf.DeviceValues = deviceValues;
        newVrf.EtalonValues = etalonValues;
        newVrf.ActualError = actualErrors;
        newVrf.ActualVariation = actualVariations;
    }
}