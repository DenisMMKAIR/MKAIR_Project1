namespace ProjApp.Normalizers;

public class ComplexStringNormalizer : IStringNormalizer
{
    private readonly IReadOnlyList<IStringNormalizer> _normalizers = [new SpaceNormalizer(), new QuoteNormalizer()];
    public string Normalize(string value)
    {
        value = value.Trim().ToUpper();
        foreach (var normalizer in _normalizers) value = normalizer.Normalize(value);
        return value;
    }
}
