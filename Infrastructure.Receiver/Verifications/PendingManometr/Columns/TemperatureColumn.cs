using Infrastructure.Receiver.ColumnsNormalizers;
using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class TemperatureColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["температура"];
    public string InternalName { get; } = nameof(PendingManometrVerification.Temperature);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
        new SingleFloatDigitColumnNormalizer()
    ];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
