using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class VerificationMethodsColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["методика поверки"];
    public string InternalName { get; } = nameof(PendingManometrVerification.VerificationMethods);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
