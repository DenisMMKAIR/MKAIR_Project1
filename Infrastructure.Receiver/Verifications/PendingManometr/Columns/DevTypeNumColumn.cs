using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class DevTypeNumColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["обозначение си"];
    public string InternalName { get; } = nameof(PendingManometrVerification.DeviceTypeNumber);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
