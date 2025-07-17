using ProjApp.Database.Commands;

namespace Project1Tests.FGISAPITests;

public class GetInitialVerificationsTests : FGISAPIFixture
{
    [Test]
    public async Task Test1()
    {
        var (good, failed) = await Client.GetInitialVerifications(new DateOnly(2024, 2, 1));

        var etalons = good
            .SelectMany(v => v.Etalons!)
            .Concat(failed.SelectMany(v => v.Etalons!))
            .Distinct(new EtalonUniqComparer())
            .ToArray();

        var devices = good
            .Select(v => v.Device!)
            .Concat(failed.Select(v => v.Device!))
            .Distinct(DeviceUniqComparer.Instance)
            .ToArray();

        var deviceTypes = good
            .Select(v => v.Device!.DeviceType!)
            .Concat(failed.Select(v => v.Device!.DeviceType!))
            .Distinct(DeviceTypeUniqComparer.Instance)
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(etalons, Has.Length.EqualTo(10));
            Assert.That(good, Has.Count.EqualTo(689));
            Assert.That(failed, Has.Count.EqualTo(0));
            Assert.That(devices, Has.Length.EqualTo(683));
            Assert.That(deviceTypes, Has.Length.EqualTo(66));
        }
    }
}
