using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class PressureColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["давление"];
    public string InternalName { get; } = nameof(PendingManometrVerification.Pressure);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
