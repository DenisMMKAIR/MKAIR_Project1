namespace Infrastructure.Receiver;

internal interface IColumn
{
    public IReadOnlyCollection<string> IncomingNames { get; }
    public string InternalName { get; }
    public IReadOnlyList<IColumnNormalizer> Normalizers { get; }
    public IReadOnlyList<IColumnVerifier> Verifiers { get; }
    public uint ColumnIndex { get; set; }
}

internal interface IColumnNormalizer
{
    public string Normalize(string value);
}

internal interface IColumnVerifier
{
    public string ErrorMessage { get; }
    public bool Verify(string value);
}
