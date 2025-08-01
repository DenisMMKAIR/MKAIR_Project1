using Infrastructure.DocumentProcessor.Services;
using Microsoft.Extensions.Configuration;

namespace Project1Tests.DocumentProcessor;

[TestFixture]
internal abstract class CombinedFixture
{
    protected TemplateProcessor Processor { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "SignsDirPath", Tools.GetSignsDirPath() }
            })
            .Build();

        Processor = new TemplateProcessor(cfg);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await Processor.DisposeAsync();
    }
}
