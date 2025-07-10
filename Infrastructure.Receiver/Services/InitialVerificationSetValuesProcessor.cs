using System.Text.RegularExpressions;
using Infrastructure.Receiver.ColumnsNormalizers;
using Infrastructure.Receiver.ColumnsVerifiers;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;

namespace Infrastructure.Receiver.Services;

public partial class InitialVerificationSetValuesProcessor : IIVSetValuesProcessor
{
    private readonly ExcelFileProcessor<InitialVerificationDataItem, IInitialVerification> _excelFileProcessor;

    public InitialVerificationSetValuesProcessor(ILogger<InitialVerificationSetValuesProcessor> logger)
    {
        _excelFileProcessor = new ExcelFileProcessor<InitialVerificationDataItem, IInitialVerification>(logger);
    }

    public IReadOnlyList<IInitialVerification> SetFromExcelFile(Stream fileStream, SetValuesRequest request)
    {
        var ivs = _excelFileProcessor.ReadVerificationFile(fileStream, string.Empty, request.SheetName, request.DataRange, new IVColumnsSetup(request), request.Location);

        return ivs;
    }

    private class IVColumnsSetup : IColumnsSetup
    {
        public IReadOnlyCollection<IColumn> Columns { get; }
        public IReadOnlyCollection<IColumnVerifier> AllColumnsVerifiers { get; }

        public IVColumnsSetup(SetValuesRequest request)
        {
            var columns = new List<IColumn>
            {
                new DeviceTypeNumberColumn(),
                new DeviceSerialColumn(),
                new VerificationDateColumn(),
            };

            if (request.VerificationTypeNum is true) columns.Add(new VerificationTypeNumColumn());
            if (request.Worker is true) columns.Add(new WorkerColumn());
            if (request.Pressure is true) columns.Add(new PressureColumn());
            if (request.Temperature is true) columns.Add(new TemperatureColumn());
            if (request.Humidity is true) columns.Add(new HumidityColumn());
            if (request.MeasurementRange is true) columns.Add(new MeasurementRangeInfoColumn());
            if (request.Accuracy is true) columns.Add(new AccuracyColumn());

            Columns = columns;
            AllColumnsVerifiers = [new NotEmptyColumnVerifier()];
        }

        private class DeviceTypeNumberColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["обозначение си", "рег. номер типа си"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.DeviceTypeNumber);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class DeviceSerialColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["заводской номер", "заводской №/ буквенно-цифровое обозначение"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.DeviceSerial);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class VerificationDateColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["дата поверки"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.VerificationDate);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class VerificationTypeNumColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["номер_протокола"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.VerificationTypeNum);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class WorkerColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["поверитель"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Worker);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new WorkerNameVerifier(),
            ];
            public uint ColumnIndex { get; set; }
        }

        private class PressureColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["давление"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Pressure);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class TemperatureColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["температура"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Temperature);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
                new SingleFloatDigitColumnNormalizer(),
            ];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class HumidityColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["влажность"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Humidity);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
                new SingleFloatDigitColumnNormalizer(),
            ];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class MeasurementRangeInfoColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["прочие сведения"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.MeasurementRange);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
                new AllSpacesRemoverColumnNormalizer(),
            ];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class AccuracyColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["класс точности", "другие параметры"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Accuracy);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
                new SingleFloatDigitColumnNormalizer(),
            ];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }
    }

    private partial class InitialVerificationDataItem : IDataItem<IInitialVerification>
    {
        public string DeviceTypeNumber { get; set; } = string.Empty;
        public string DeviceSerial { get; set; } = string.Empty;
        public DateOnly VerificationDate { get; set; }
        public string? VerificationTypeNum { get; set; }
        public string? Worker { get; set; }
        public string? Pressure { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public string? MeasurementRange { get; set; }
        public string? Accuracy { get; set; }

        public IInitialVerification PostProcess(string fileName, int rowNumber, DeviceLocation location)
        {
            var additionalInfo = new Dictionary<string, object>();

            if (MeasurementRange != null)
            {
                var measurementMatch = MeasurementRegex().Match(MeasurementRange);
                if (!measurementMatch.Success)
                {
                    throw new InvalidDataException($"Файл {fileName}. Строка {rowNumber}. Не удалось распознать диапазон измерений. Введенная строка {MeasurementRange}");
                }
                additionalInfo["min"] = int.Parse(measurementMatch.Groups[1].Value);
                additionalInfo["max"] = int.Parse(measurementMatch.Groups[2].Value);
                additionalInfo["unit"] = int.Parse(measurementMatch.Groups[3].Value);
            }

            if (Accuracy! != null)
            {
                if (!double.TryParse(Accuracy, out var resultAccuracy))
                {
                    throw new InvalidDataException($"Файл {fileName}. Строка {rowNumber}. Не удалось распознать точность. Введенная строка {Accuracy}");
                }
                additionalInfo["accuracy"] = resultAccuracy;
            }

            return new SuccessInitialVerification()
            {
                DeviceTypeNumber = DeviceTypeNumber,
                DeviceSerial = DeviceSerial,
                VerificationDate = VerificationDate,
                Location = location,
                ProtocolNumber = VerificationTypeNum,
                Worker = Worker,
                AdditionalInfo = additionalInfo,
                Pressure = Pressure,
                Temperature = Temperature,
                Humidity = Humidity,

                VerifiedUntilDate = default,
                VerificationTypeName = string.Empty,
                Owner = string.Empty,
            };
        }

        [GeneratedRegex(@"\(([+-]*\d+)-([+-]*\d+)\)(.+)")]
        private static partial Regex MeasurementRegex();
    }
}
