using System.ComponentModel.DataAnnotations;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class ProtocolTemplateController : ApiControllerBase
{
    private readonly ProtocolTemplesService _protocolTemplesService;
    private readonly IMapper _mapper;

    public ProtocolTemplateController(ProtocolTemplesService protocolTemplesService, IMapper mapper)
    {
        _protocolTemplesService = protocolTemplesService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<ProtocolTemplateDTO>> GetTemplates([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _protocolTemplesService.GetProtocolsAsync(request.PageIndex, request.PageSize);
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<PossibleTemplateVerificationMethodsDTO>> GetPossibleVerificationMethods([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _protocolTemplesService.GetPossibleVerificationMethodsAsync(request.PageIndex, request.PageSize);
    }

    [HttpPost]
    public async Task<ServiceResult> AddTemplate([Required][FromForm] AddProtocolTemplateRequest request)
    {
        return await _protocolTemplesService.AddProtocolAsync(request.Adapt<ProtocolTemplate>(_mapper.Config));
    }

    [HttpPatch]
    public async Task<ServiceResult> AddVerificationMethod([Required][FromForm] AddProtocolVerificationMethodRequest request)
    {
        return await _protocolTemplesService.AddVerificationMethodAsync(request.TemplateId, request.VerificationMethodId);
    }

    public record AddProtocolVerificationMethodRequest(Guid TemplateId, Guid VerificationMethodId);

    [HttpDelete]
    public async Task<ServiceResult> DeleteTemplate([Required][FromQuery] Guid id)
    {
        return await _protocolTemplesService.DeleteProtocolAsync(id);
    }
}
