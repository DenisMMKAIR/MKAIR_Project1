using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class WorkerNameColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["поверитель"];
    public string InternalName { get; } = nameof(PendingManometrVerification.WorkerName);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
