using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, string FileName, byte[] FileContent)
{
    public VerificationMethod MapToVerificationMethod()
    {
        return new()
        {
            Description = Description,
            Aliases = [.. Aliases.Select(a => new VerificationMethodAlias { Name = a })],
            FileName = FileName,
            FileContent = FileContent
        };
    }
}
