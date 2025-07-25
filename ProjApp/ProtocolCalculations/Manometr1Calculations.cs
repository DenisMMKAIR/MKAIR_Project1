using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.ProtocolCalculations;

internal static class Manometr1Calculations
{
    public static Manometr1Verification ToManometr1(ILogger logger, SuccessInitialVerification v, Owner owner)
    {
        if (v.Device is null) throw new Exception("Устройство не добавлено в выборку");
        if (v.Device.DeviceType is null) throw new Exception("Тип устройства не добавлен в выборку");
        if (v.VerificationMethod is null) throw new Exception("Метод проверки не добавлен в выборку");

        var method = v.VerificationMethod;

        var newVrf = new Manometr1Verification
        {
            ProtocolNumber = v.ProtocolNumber!,
            Temperature = v.Temperature!.Value,
            Humidity = v.Humidity!.Value,
            Pressure = v.Pressure!,
            VisualCheckup = method.Checkups[VerificationMethodCheckups.внешний_осмотр],
            TestCheckup = method.Checkups[VerificationMethodCheckups.результат_опробывания],
            AccuracyCalculation = method.Checkups[VerificationMethodCheckups.опр_осн_поргрешности],
            VerificationDate = v.VerificationDate,
            Worker = v.Worker!,

            // Support properties
            DeviceTypeNumber = v.DeviceTypeNumber,
            DeviceSerial = v.DeviceSerial,
            VerificationGroup = v.VerificationGroup!.Value,
            Location = v.Location!.Value,
            VerifiedUntilDate = v.VerifiedUntilDate,
            InitialVerificationName = v.VerificationTypeName,
            OwnerInitialName = v.Owner,

            // Table values
            MeasurementMin = v.MeasurementMin!.Value,
            MeasurementMax = v.MeasurementMax!.Value,
            MeasurementUnit = v.MeasurementUnit!,
            ValidError = v.Accuracy!.Value,

            DeviceValues = default!,
            EtalonValues = default!,
            ActualError = default!,
            ActualVariation = default!,

            // Navigation properties
            Device = v.Device,
            VerificationMethod = v.VerificationMethod,
            Owner = owner,
            Etalons = v.Etalons,
        };

        CalculateValues(logger, newVrf);

        return newVrf;
    }

    private static void CalculateValues(ILogger logger, Manometr1Verification newVrf)
    {
        var magicValues = new double[] { 4, 8, 12, 16, 20, 24, 28, 32 };

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