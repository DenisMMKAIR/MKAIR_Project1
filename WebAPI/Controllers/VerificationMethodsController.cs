using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Mapping;
using ProjApp.Services;
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
    public async Task<ServicePaginatedResult<PossibleVerificationMethodDTO>> GetPossibleVerificationMethods([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetPossibleVerificationMethodsAsync(request.PageIndex, request.PageSize);
    }

    [HttpPost]
    public async Task<ServiceResult> AddVerificationMethod([Required][FromForm] AddVerificationMethodRequest request)
    {
        return await _service.AddVerificationMethodAsync(request.MapToVerificationMethod());
    }
}
