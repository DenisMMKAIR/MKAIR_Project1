namespace Project1Tests;

internal static class Samples
{
    public static string Manometr1Path { get; } = "Manometr.html".GetFormPathFromDocumentProcessor();
    public static string Poverki1Path { get; } = "поверки.xlsm".GetSamplePath();
    public static string Poverki2Path { get; } = "поверки2.xlsm".GetSamplePath();
    public static string Poverki3Path { get; } = "поверки3.xlsm".GetSamplePath();
    public static string PoverkiFloatDigitsTestPath { get; } = "поверки float digits tests.xlsm".GetSamplePath();
}
