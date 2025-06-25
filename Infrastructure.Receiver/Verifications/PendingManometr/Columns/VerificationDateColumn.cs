using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class VerificationDateColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["дата поверки"];
    public string InternalName { get; } = nameof(PendingManometrVerification.Date);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
