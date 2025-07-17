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
      Meth = response.Meth?
        .Select(meth => new DeviceType.MethodClass { Title = meth.Title, DocUrl = meth.Doc_URL })
        .ToArray(),
      Spec = response.Spec?
        .Select(spec => new DeviceType.SpecClass { DocUrl = spec.Doc_URL })
        .ToArray(),
      Manufacturer = response.Manufacturer?
        .Select(man => new DeviceType.ManufacturerClass { Title = man.Title })
        .ToArray()
    };
  }
}