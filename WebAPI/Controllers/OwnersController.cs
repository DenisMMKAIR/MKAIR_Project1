using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class OwnersController : ApiControllerBase
{
    private readonly OwnersService _service;

    public OwnersController(OwnersService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<Owner>> GetOwners([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetOwners(request.PageIndex, request.PageSize);
    }

    [HttpPost]
    public async Task<ServiceResult> AddOwner([Required][FromForm] OwnerRequest request)
    {
        return await _service.AddOwner(new() { Name = request.Name, INN = request.INN });
    }

    [HttpPatch]
    public async Task<ServiceResult> SetOwnerINN([Required][FromForm] SetOwnerINNRequest request)
    {
        return await _service.SetOwnerINN(request.Id, request.INN);
    }

    public record OwnerRequest(string Name, ulong INN);
    public record SetOwnerINNRequest(Guid Id, ulong INN);
}
