using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, string FileName, IFormFile File)
{
    public VerificationMethod MapToVerificationMethod()
    {
        if (File.Length > 10 * 1024 * 1024) throw new Exception("Файл слишком большой. Лимит 10мб"); // 10 MB
        var ms = new MemoryStream();
        File.CopyTo(ms);
        return new()
        {
            Description = Description,
            Aliases = [.. Aliases.Select(a => new VerificationMethodAlias { Name = a })],
            FileName = FileName,
            FileContent = ms.ToArray()
        };
    }
}
