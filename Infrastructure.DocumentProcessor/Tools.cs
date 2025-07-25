namespace Infrastructure.DocumentProcessor;

internal static class Tools
{
    public static string GetFormPath(this string fileName) => Path.Combine(AppContext.BaseDirectory, "Forms", fileName);
}
