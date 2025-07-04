namespace ProjApp.Database.Normalizers;

public class ComplexStringNormalizer : IStringNormalizer
{
    private readonly IReadOnlyList<IStringNormalizer> _normalizers = [new SpaceNormalizer(), new QuoteNormalizer()];
    public string Normalize(string value)
    {
        foreach (var normalizer in _normalizers) value = normalizer.Normalize(value);
        value = value.Trim().ToUpper();
        return value;
    }
}
