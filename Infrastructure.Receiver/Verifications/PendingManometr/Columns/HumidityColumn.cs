using Infrastructure.Receiver.ColumnsNormalizers;
using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class HumidityColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["влажность"];
    public string InternalName { get; } = nameof(PendingManometrVerification.Hummidity);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
        new SingleFloatDigitColumnNormalizer()
    ];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
