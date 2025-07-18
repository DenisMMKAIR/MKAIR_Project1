namespace ProjApp.Normalizers;

public class OwnerNameNormalizer : IStringNormalizer
{
    public string Normalize(string value)
    {
        return SpaceNormalizer.Instance.Normalize(value).Trim().ToUpper();
    }

    public static IStringNormalizer Instance { get; } = new OwnerNameNormalizer();
}