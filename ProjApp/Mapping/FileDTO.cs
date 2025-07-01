namespace ProjApp.Mapping;

public class FileDTO
{
    public string FileName { get; init; }
    public string Mimetype { get; init; }
    public byte[] FileContent { get; init; }

    public FileDTO(string fileName, byte[] fileContent)
    {
        FileName = fileName;
        FileContent = fileContent;
        Mimetype = SetMimetype(fileName);
    }

    private static readonly Dictionary<string, string> _mimetypes = new(){
        { ".pdf", "application/pdf" }
    };

    private static string SetMimetype(string fileName)
    {
        var fileExt = Path.GetExtension(fileName);
        return _mimetypes.TryGetValue(fileExt, out var mimeType)
            ? mimeType
            : "application/octet-stream";
    }
}