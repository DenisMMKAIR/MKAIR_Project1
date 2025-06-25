using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class OwnerNameColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["владелец си"];
    public string InternalName { get; } = nameof(PendingManometrVerification.OwnerName);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
