namespace ProjApp.Normalizers.VerificationMethod;

public class VerificationMethodDescriptionNormalizer : IStringNormalizer
{
    public string Normalize(string value)
    {
        return SpaceNormalizer.Instance.Normalize(value);
    }

    public static IStringNormalizer Instance { get; } = new VerificationMethodDescriptionNormalizer();
}
