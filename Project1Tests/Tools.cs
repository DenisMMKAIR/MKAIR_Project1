namespace Project1Tests;

internal static class Tools
{
    public static string GetProjectDirPath(this string dirName)
    {
        return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", dirName));
    }

    public static string GetSamplePath(this string fileName)
    {
        return Path.Combine("Samples".GetProjectDirPath(), fileName);
    }

    public static string GetFormPathFromDocumentProcessor(this string fileName)
    {
        var fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", $"{nameof(DocumentProcessor)}"));
        return Path.Combine(fullPath, "Forms", fileName);
    }

    public static string GetSignsDirPath()
    {
        return Path.Combine("Samples".GetProjectDirPath(), "Подписи поверителей");
    }

    public static (MemoryStream fileContent, string FileName) ToFileStream(this string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var fileContent = new MemoryStream();
        using var fs = File.OpenRead(filePath);
        fs.CopyTo(fileContent);

        return (fileContent, fileName);
    }
}
