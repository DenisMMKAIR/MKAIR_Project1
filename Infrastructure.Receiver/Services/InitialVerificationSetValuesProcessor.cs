using Infrastructure.Receiver.ColumnsNormalizers;
using Infrastructure.Receiver.ColumnsVerifiers;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;
using ProjApp.Services;

namespace Infrastructure.Receiver.Services;

public class InitialVerificationSetValuesProcessor : IIVSetValuesProcessor
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
            if (request.AdditionalInfo is true) columns.Add(new AdditionalInfoColumn());
            if (request.Pressure is true) columns.Add(new PressureColumn());
            if (request.Temperature is true) columns.Add(new TemperatureColumn());
            if (request.Humidity is true) columns.Add(new HumidityColumn());

            Columns = columns;
            AllColumnsVerifiers = [];
        }

        private class DeviceTypeNumberColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["обозначение си", "рег. номер типа си"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.DeviceTypeNumber);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class DeviceSerialColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["заводской номер", "заводской №/ буквенно-цифровое обозначение"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.DeviceSerial);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class VerificationDateColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["дата поверки"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.VerificationDate);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class VerificationTypeNumColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["номер_протокола"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.VerificationTypeNum);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class WorkerColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["поверитель"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Worker);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier(),
                new NameVerifier(),
            ];
            public uint ColumnIndex { get; set; }
        }

        private class AdditionalInfoColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["класс точности", "другие параметры"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.AdditionalInfo);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class PressureColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["давление"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Pressure);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class TemperatureColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["температура"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Temperature);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
                new SingleFloatDigitColumnNormalizer(),
            ];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }

        private class HumidityColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["влажность"];
            public string InternalName { get; } = nameof(InitialVerificationDataItem.Humidity);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
                new SingleFloatDigitColumnNormalizer(),
            ];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [
                new NotEmptyColumnVerifier()
            ];
            public uint ColumnIndex { get; set; }
        }
    }

    private class InitialVerificationDataItem : IDataItem<IInitialVerification>
    {
        public string DeviceTypeNumber { get; set; } = string.Empty;
        public string DeviceSerial { get; set; } = string.Empty;
        public DateOnly VerificationDate { get; set; }
        public string? VerificationTypeNum { get; set; }
        public string? Worker { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? Pressure { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }

        public IInitialVerification PostProcess(string fileName, int rowNumber, DeviceLocation location)
        {
            return new InitialVerification()
            {
                DeviceTypeNumber = DeviceTypeNumber,
                DeviceSerial = DeviceSerial,
                VerificationDate = VerificationDate,
                Location = location,
                VerificationTypeNum = VerificationTypeNum,
                Worker = Worker,
                AdditionalInfo = AdditionalInfo,
                Pressure = Pressure,
                Temperature = Temperature,
                Humidity = Humidity,

                VerifiedUntilDate = default,
                VerificationTypeNames = [],
                Owner = string.Empty,
            };
        }
    }
}
