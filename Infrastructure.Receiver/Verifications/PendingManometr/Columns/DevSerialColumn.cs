using Infrastructure.Receiver.ColumnsNormalizers;
using ProjApp.Database.Entities;

namespace Infrastructure.Receiver.Verifications.PendingManometr.Columns;

internal class DevSerialColumn : IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; } = ["заводской номер"];
    public string InternalName { get; } = nameof(PendingManometrVerification.DeviceSerial);
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; } = [
        new LeadingZerosColumnNormalizer()
    ];
    public IReadOnlyList<IColumnVerifier> Verifiers { get; } = [];
    public uint ColumnIndex { get; set; }
}
