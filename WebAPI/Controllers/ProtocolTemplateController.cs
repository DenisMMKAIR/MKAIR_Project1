using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class ProtocolTemplateController : ApiControllerBase
{
    private readonly ProtocolTemplesService _protocolTemplesService;

    public ProtocolTemplateController(ProtocolTemplesService protocolTemplesService)
    {
        _protocolTemplesService = protocolTemplesService;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<ProtocolTemplateDTO>> GetTemplates([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _protocolTemplesService.GetProtocolsAsync(request.PageIndex, request.PageSize);
    }

    [HttpPost]
    public async Task<ServiceResult> AddTemplate([Required][FromForm] AddProtocolTemplateRequest request)
    {
        return await _protocolTemplesService.AddProtocolAsync(AddProtocolTemplateRequest.ToProtocolTemplate(request));
    }

}
