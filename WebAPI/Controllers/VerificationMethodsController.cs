using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
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
    public async Task<ServicePaginatedResult<VerificationMethod>> GetVerificationMethods([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetVerificationMethodsAsync(request.PageIndex, request.PageSize);
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<PossibleVerificationMethodDTO>> GetPossibleVerificationMethods([Required][FromQuery] GetPaginatedRequest request, string? search)
    {
        return await _service.GetPossibleVerificationMethodsAsync(request.PageIndex, request.PageSize, search);
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
            FileName = request.FileName,
            FileContent = fileContent
        };
        return await _service.AddVerificationMethodAsync(newVerMethod);
    }
}
