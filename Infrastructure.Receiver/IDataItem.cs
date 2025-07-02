using ProjApp.Database.EntitiesStatic;

namespace Infrastructure.Receiver;

internal interface IDataItem<T>
{
    public T PostProcess(string fileName, int rowNumber, DeviceLocation location);
}
