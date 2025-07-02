using ProjApp.Database.EntitiesStatic;

namespace WebAPI.Controllers.Requests;

public record ExcelVerificationsRequest(IFormFile File, string SheetName, string DataRange, DeviceLocation DeviceLocation);