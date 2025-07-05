using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class InitialVerificationsController : ApiControllerBase
{
    private readonly InitialVerificationService _service;

    public InitialVerificationsController(InitialVerificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<InitialVerificationDto>> GetVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] GetVerificationsRequest? query)
    {
        return await _service.GetInitialVerifications(request.PageIndex, request.PageSize, YearMonth.Parse(query?.VerificationYearMonth));
    }

    [HttpPatch]
    public Task<ServiceResult> AddValues([Required][FromForm] AddValuesRequest request)
    {

        throw new NotImplementedException();
    }

    public class AddValuesRequest
    {
        public required IFormFile ExcelFile { get; init; }
        public bool VerificationTypeNum { get; init; }
        public bool OwnerInn { get; init; }
        public bool Worker { get; init; }
        public bool Location { get; init; }
        public bool AdditionalInfo { get; init; }
        public bool Pressure { get; init; }
        public bool Temperature { get; init; }
        public bool Humidity { get; init; }
    }

    public class GetVerificationsRequest
    {
        public string? VerificationYearMonth { get; set; }
    }
}

/*

    public string?  { get; set; }
    public uint? OwnerInn { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    */