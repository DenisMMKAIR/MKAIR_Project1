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
    public async Task<ServicePaginatedResult<PossibleProtocolTemplateResultDTO>> GetPossibleTemplates([Required][FromQuery] GetPaginatedRequest request, [Required][FromQuery] bool success)
    {
        return await _protocolTemplesService.GetPossibleTemplatesAsync(request.PageIndex, request.PageSize, success);
    }

    [HttpPost]
    public async Task<ServiceResult> AddTemplate([Required][FromBody] AddProtocolTemplateRequest request)
    {
        return await _protocolTemplesService.AddProtocolAsync(request.Adapt<ProtocolTemplate>(_mapper.Config));
    }

    [HttpDelete]
    public async Task<ServiceResult> DeleteTemplate([Required][FromQuery] int id)
    {
        return await _protocolTemplesService.DeleteProtocolAsync(id);
    }
}
