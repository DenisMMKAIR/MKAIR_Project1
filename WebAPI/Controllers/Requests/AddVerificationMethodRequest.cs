namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, string FileName, IFormFile File);