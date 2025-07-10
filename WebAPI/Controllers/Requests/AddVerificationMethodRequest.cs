namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, Dictionary<string,string> Checkups, string FileName, IFormFile File);