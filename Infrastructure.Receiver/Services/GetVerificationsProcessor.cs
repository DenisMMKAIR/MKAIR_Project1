using Infrastructure.Receiver.ColumnsVerifiers;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.InfrastructureInterfaces;

namespace Infrastructure.Receiver.Services;

public class GetVerificationsProcessor : IGetVerificationsFromExcelProcessor
{
    private readonly ExcelFileProcessor<ExcelData, IInitialVerification> _excelFileProcessor;

    public GetVerificationsProcessor(ILogger<GetVerificationsProcessor> logger)
    {
        _excelFileProcessor = new ExcelFileProcessor<ExcelData, IInitialVerification>(logger); ;
    }

    public IReadOnlyList<IInitialVerification> GetVerificationsFromFile(Stream fileStream, string fileName, string sheetName, string dataRange)
    {
        return _excelFileProcessor.ReadVerificationFile(
            fileStream,
            fileName,
            sheetName,
            dataRange,
            new GetVerificationsColumnsSetup(),
            DeviceLocation.ГПНЯмал);
    }

    private class GetVerificationsColumnsSetup : IColumnsSetup
    {
        public IReadOnlyCollection<IColumn> Columns { get; }
        public IReadOnlyCollection<IColumnVerifier> AllColumnsVerifiers { get; }

        public GetVerificationsColumnsSetup()
        {
            Columns =
            [
                new DeviceTypeNumberColumn(),
                new DeviceSerialColumn(),
                new VerificationDateColumn(),
            ];

            AllColumnsVerifiers = [new NotEmptyColumnVerifier()];
        }

        private class DeviceTypeNumberColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["обозначение си", "рег. номер типа си"];
            public string InternalName { get; } = nameof(ExcelData.DeviceTypeNumber);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class DeviceSerialColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["заводской номер", "заводской №/ буквенно-цифровое обозначение"];
            public string InternalName { get; } = nameof(ExcelData.DeviceSerial);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }

        private class VerificationDateColumn : IColumn
        {
            public IReadOnlyCollection<string> IncomingNames { get; } = ["дата поверки"];
            public string InternalName { get; } = nameof(ExcelData.VerificationDate);
            public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
            public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
            public uint ColumnIndex { get; set; }
        }
    }

    private partial class ExcelData : IDataItem<IInitialVerification>
    {
        public string DeviceTypeNumber { get; set; } = string.Empty;
        public string DeviceSerial { get; set; } = string.Empty;
        public DateOnly VerificationDate { get; set; }

        public IInitialVerification PostProcess(string fileName, int rowNumber, DeviceLocation location)
        {
            return new SuccessInitialVerification
            {
                DeviceTypeNumber = DeviceTypeNumber,
                DeviceSerial = DeviceSerial,
                VerificationDate = VerificationDate,

                Owner = string.Empty,
                VerificationTypeName = string.Empty,
                VerifiedUntilDate = default,
            };
        }
    }
}
