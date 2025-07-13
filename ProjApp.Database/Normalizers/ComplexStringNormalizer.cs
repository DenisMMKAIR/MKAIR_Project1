namespace ProjApp.Database.Normalizers;

public class ComplexStringNormalizer : IStringNormalizer
{
    private readonly IReadOnlyList<IStringNormalizer> _normalizers = [
        new QuoteNormalizer(),
        new SpaceNormalizer(),
    ];

    public string Normalize(string value)
    {
        foreach (var normalizer in _normalizers) value = normalizer.Normalize(value);
        value = value.ToUpper();
        return value;
    }
}
