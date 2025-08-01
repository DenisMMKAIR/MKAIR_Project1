namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, Dictionary<string, string> Checkups);

public record VerificationMethodFileRequest(string? FileName, IFormFile? File);