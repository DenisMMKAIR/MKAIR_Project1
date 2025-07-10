namespace Infrastructure.Receiver.ColumnsNormalizers;

internal class DotToCommaNormalizer : IColumnNormalizer
{
    public string Normalize(string value)
    {
        return value.Replace('.', ',');
    }
}
