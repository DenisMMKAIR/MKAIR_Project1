namespace Infrastructure.Receiver;

internal interface IColumnsSetup
{
    public IReadOnlyCollection<IColumn> Columns { get; }
    public IReadOnlyCollection<IColumnVerifier> AllColumnsVerifiers { get; }
}
