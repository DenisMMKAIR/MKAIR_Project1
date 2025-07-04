using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class VerificationMethodsController : ApiControllerBase
{
    private readonly VerificationMethodsService _service;

    public VerificationMethodsController(VerificationMethodsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<VerificationMethodDTO>> GetVerificationMethods([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetVerificationMethodsAsync(request.PageIndex, request.PageSize);
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<PossibleVerificationMethodDTO>> GetPossibleVerificationMethods([Required][FromQuery] GetPaginatedRequest request, [FromQuery] PossibleVerificationMethodRequest query)
    {
        return await _service.GetPossibleVerificationMethodsAsync(request.PageIndex, request.PageSize, query.VerificationNameFilter, query.DeviceTypeInfoFilter, YearMonth.Parse(query.YearMonthFilter));
    }

    public class PossibleVerificationMethodRequest
    {
        public string? VerificationNameFilter { get; init; }
        public string? DeviceTypeInfoFilter { get; init; }
        public string? YearMonthFilter { get; init; }
    }

    [HttpGet]
    public async Task<IActionResult> DownloadFile([Required][FromQuery] Guid fileId)
    {
        var fileResult = await _service.DownloadFileAsync(fileId);
        if (fileResult.Error != null) return BadRequest(fileResult.Error);
        return File(fileResult.Item!.Content, fileResult.Item.Mimetype, fileResult.Item.FileName);
    }

    [HttpPost]
    public async Task<ServiceResult> AddVerificationMethod([Required][FromForm] AddVerificationMethodRequest request)
    {
        if (request.File.Length > 10 * 1024 * 1024) return ServiceResult.Fail("Не удалось загрузить файл. Лимит 10 МБ");

        using var ms = new MemoryStream();
        request.File.CopyTo(ms);
        var fileContent = ms.ToArray();

        var newVerMethod = new VerificationMethod
        {
            Aliases = request.Aliases,
            Description = request.Description,
            VerificationMethodFiles = [new() { FileName = request.FileName, Mimetype = SetMimetype(request.FileName), Content = fileContent }]
        };
        return await _service.AddVerificationMethodAsync(newVerMethod);
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
