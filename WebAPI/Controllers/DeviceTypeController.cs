using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class DeviceTypeController : ApiControllerBase
{
    private readonly DeviceTypeService _deviceTypeService;

    public DeviceTypeController(DeviceTypeService deviceTypeService)
    {
        _deviceTypeService = deviceTypeService;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<DeviceType>> GetDevicesPaginated([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _deviceTypeService.GetPaginatedAsync(request.PageIndex, request.PageSize);
    }

    [HttpPost]
    public async Task<ServiceResult> AddDeviceType([Required][FromForm] AddDeviceTypeRequest request)
    {
        return await _deviceTypeService.AddDeviceTypeAsync(request.ToDeviceType());
    }
}
