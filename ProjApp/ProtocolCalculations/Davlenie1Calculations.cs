using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;

namespace ProjApp.ProtocolCalculations;

internal static class Davlenie1Calculations
{
    public static Davlenie1Verification ToDavlenie1(ILogger logger, SuccessInitialVerification v, Owner owner)
    {
        if (v.Device is null) throw new Exception("Устройство не добавлено в выборку");
        if (v.Device.DeviceType is null) throw new Exception("Тип устройства не добавлен в выборку");
        if (v.VerificationMethod is null) throw new Exception("Метод проверки не добавлен в выборку");

        var newVrf = new Davlenie1Verification()
        {
            ProtocolNumber = v.ProtocolNumber!,
            Temperature = v.Temperature!.Value,
            Humidity = v.Humidity!.Value,
            Pressure = v.Pressure!,
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
            PressureInputs = [],
            EtalonValues = [],
            DeviceValues = [],
            ActualError = [],
            ValidError = v.Accuracy!.Value,
            Variations = [],

            // Navigation properties
            Device = v.Device,
            VerificationMethod = v.VerificationMethod,
            Owner = owner,
            Etalons = v.Etalons
        };

        CalculateValues(logger, newVrf);

        return newVrf;
    }

    private static void CalculateValues(ILogger logger, Davlenie1Verification newVrf)
    {
        var min = newVrf.MeasurementMin;
        var max = newVrf.MeasurementMax;
        var error = newVrf.ValidError;

        var etalonValues = newVrf.EtalonValues = [4, 8, 12, 16, 20];
        double[] pressureInputs = [0, 0, 0, 0, 0];
        double[][] deviceValues = [[0, 0, 0, 0, 0], [0, 0, 0, 0, 0]];
        double[][] actualErrors = [[0, 0, 0, 0, 0], [0, 0, 0, 0, 0]];
        double[] variations = [0, 0, 0, 0, 0];

        var errorValue = etalonValues[0] / 100 * error;

        for (int i = 0; i < etalonValues.Count; i++)
        {
            var etalonValue = etalonValues[i];
            var pressureInput = pressureInputs[i] = (max - min) / 100 * ((etalonValue - 4) / 16 * 100) + min;
            var minRange = etalonValue - errorValue;
            var maxRange = etalonValue + errorValue;
            var forwardValue = deviceValues[0][i] = minRange + (maxRange - minRange) * Random.Shared.NextDouble();
            var forwardActualError = actualErrors[0][i] = (forwardValue - etalonValue) / 16 * 100;
            if (Math.Abs(forwardActualError) > error)
            {
                var msg = $"У {newVrf} ошибка прямого хода {forwardActualError:N3} превышает допустимую {error:N3}";
                logger.LogError("{Msg}", msg);
                throw new Exception(msg);
            }
            var backwardValue = deviceValues[1][i] = minRange + (maxRange - minRange) * Random.Shared.NextDouble();
            var backwardActualError = actualErrors[1][i] = (backwardValue - etalonValue) / 16 * 100;
            if (Math.Abs(backwardActualError) > error)
            {
                var msg = $"У {newVrf} ошибка обратного хода {backwardActualError:N3} превышает допустимую {error:N3}";
                logger.LogError("{Msg}", msg);
                throw new Exception(msg);
            }
            var variation = variations[i] = (forwardValue - backwardValue) / 16 * 100;
            if (Math.Abs(variation) > error)
            {
                var msg = $"У {newVrf} ошибка вариации {variation:N3} превышает допустимую {error:N3}";
                logger.LogError("{Msg}", msg);
                throw new Exception(msg);
            }
        }

        newVrf.PressureInputs = pressureInputs;
        newVrf.DeviceValues = deviceValues;
        newVrf.ActualError = actualErrors;
        newVrf.Variations = variations;
    }
}
