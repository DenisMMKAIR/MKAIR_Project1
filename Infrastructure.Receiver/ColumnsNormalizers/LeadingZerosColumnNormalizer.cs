namespace Infrastructure.Receiver.ColumnsNormalizers;

internal class LeadingZerosColumnNormalizer : IColumnNormalizer
{
    public string Normalize(string value)
    {
        return value.TrimStart('0');
    }
}
