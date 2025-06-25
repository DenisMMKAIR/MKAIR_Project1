using Infrastructure.Receiver.Verifications.PendingManometr.Columns;

namespace Infrastructure.Receiver.Verifications.PendingManometr;

internal class PendingManometrVerificationsColumnsSetup : IColumnsSetup
{
    public IReadOnlyCollection<IColumn> Columns { get; } =
    [
        new DevTypeNumColumn(),
        new DevSerialColumn(),
        new VerificationDateColumn(),
        new VerificationMethodsColumn(),
        new EtalonsNumberColumn(),
        new OwnerNameColumn(),
        new WorkerNameColumn(),
        new TemperatureColumn(),
        new PressureColumn(),
        new HumidityColumn(),
        new AccuracyColumn(),
    ];

    public IReadOnlyCollection<IColumnVerifier> AllColumnsVerifiers { get; } = [];
}
