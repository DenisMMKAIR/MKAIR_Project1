using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class EtalonsNumberColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["си, применяемые в качестве эталона"];
    public string InternalName { get; } = nameof(PendingManometrVerification.EtalonsNumbers);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
