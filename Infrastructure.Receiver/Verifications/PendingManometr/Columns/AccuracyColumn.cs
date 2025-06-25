using Infrastructure.Receiver.ColumnsNormalizers;
using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class AccuracyColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["класс точности", "другие параметры"];
    public string InternalName { get; } = nameof(PendingManometrVerification.Accuracy);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
        new SingleFloatDigitColumnNormalizer(),
    ];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
