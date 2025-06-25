using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsNormalizers;

internal partial class AllSpacesRemoverColumnNormalizer : IColumnNormalizer
{
    public string Normalize(string value)
    {
        return MyRegex().Replace(value, "");
    }

    [GeneratedRegex(@"[ ]+")]
    private static partial Regex MyRegex();
}
