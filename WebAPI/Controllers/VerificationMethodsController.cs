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
        YearMonth? yearMonth;
        try
        {
            yearMonth = YearMonth.Parse(query.YearMonthFilter);
        }
        catch (Exception e)
        {
            return ServicePaginatedResult<PossibleVerificationMethodDTO>.Fail(e.Message);
        }
        return await _service.GetPossibleVerificationMethodsAsync(request.PageIndex, request.PageSize, query.DeviceTypeNumberFilter, query.VerificationNameFilter, query.DeviceTypeInfoFilter, yearMonth);
    }

    [HttpPost]
    public async Task<ServiceResult> AddVerificationMethod([FromForm] VerificationMethodFileRequest? fileForm, [Required][FromForm] AddVerificationMethodRequest request)
    {
        VerificationMethodFile? methodFile = null;

        if (fileForm != null && fileForm.File != null && fileForm.FileName != null)
        {
            // TODO: Setup filesize at web config instead
            if (fileForm.File.Length > 10 * 1024 * 1024) return ServiceResult.Fail("Не удалось загрузить файл. Лимит 10 МБ");

            using var ms = new MemoryStream();
            fileForm.File.CopyTo(ms);
            var fileContent = ms.ToArray();

            var mimeType = SetMimetype(fileForm.FileName);
            if (string.IsNullOrWhiteSpace(mimeType)) return ServiceResult.Fail("Не удалось определить тип файла");

            methodFile = new() { FileName = fileForm.FileName, Mimetype = mimeType, Content = fileContent };
        }

        var newVerMethod = new VerificationMethod
        {
            Aliases = request.Aliases,
            Description = request.Description,
            Checkups = request.Checkups,
            VerificationMethodFiles = methodFile != null ? [methodFile] : null,
        };

        return await _service.AddVerificationMethodAsync(newVerMethod);
    }

    [HttpPatch]
    public async Task<ServiceResult> AddAliases([Required][FromForm] AddAliasesRequest request)
    {
        return await _service.AddAliasesAsync(request.Aliases, request.VerificationMethodId);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadFile([Required][FromQuery] Guid fileId)
    {
        var fileResult = await _service.DownloadFileAsync(fileId);
        if (fileResult.Error != null) return BadRequest(fileResult.Error);
        return File(fileResult.Item!.Content, fileResult.Item.Mimetype, fileResult.Item.FileName);
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
