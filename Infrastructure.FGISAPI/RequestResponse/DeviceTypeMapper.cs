using Infrastructure.FGIS.Database.Entities;

namespace Infrastructure.FGISAPI.RequestResponse;

internal static class DeviceTypeMapper
{
  public static DeviceType ToDeviceType(this DeviceTypeResponse response)
  {
    return new DeviceType
    {
      Id = response.General.MIT_UUID,
      Number = response.General.Number,
      Title = response.General.Title,
      Notation = response.General.Notation,
      MethUrls = response.Meth?
        .Select(meth => meth.Doc_URL!)
        .Where(url => url != null)
        .ToArray(),
      SpecUrls = response.Spec?
        .Select(spec => spec.Doc_URL!)
        .Where(url => url != null)
        .ToArray(),
      Manufacturers = response.Manufacturer?
        .Select(man => man.Title!)
        .Where(man => man != null)
        .ToArray()
    };
  }
}