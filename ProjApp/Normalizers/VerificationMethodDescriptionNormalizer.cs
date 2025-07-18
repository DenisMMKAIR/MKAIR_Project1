namespace ProjApp.Normalizers;

public class VerificationMethodDescriptionNormalizer : IStringNormalizer
{
    public string Normalize(string value)
    {
        return SpaceNormalizer.Instance.Normalize(value).Trim();
    }

    public static IStringNormalizer Instance { get; } = new VerificationMethodDescriptionNormalizer();
}