using Infrastructure.Receiver;

namespace Project1Tests.Receiver.Columns;

[TestFixture]
public class ColumnsTests
{
    [Test]
    public void AllColumns_ShouldHaveUniqueIncomingNames()
    {
        var columnTypes = typeof(IColumn).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IColumn).IsAssignableFrom(t))
            .ToList();

        var nameToTypesMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in columnTypes)
        {
            var column = Activator.CreateInstance(type) as IColumn;
            foreach (var name in column!.IncomingNames)
            {
                if (!nameToTypesMap.TryGetValue(name, out List<string>? value))
                {
                    value = [];
                    nameToTypesMap[name] = value;
                }

                value.Add(type.Name);
            }
        }

        var duplicates = nameToTypesMap
            .Where(pair => pair.Value.Count > 1)
            .ToList();

        if (duplicates.Count != 0)
        {
            var errorMessage = "Найдены повторяющиеся IncomingNames:\n" +
                string.Join("\n", duplicates.Select(d =>
                    $"Имя '{d.Key}' используется в: {string.Join(", ", d.Value)}"));

            Assert.Fail(errorMessage);
        }
    }
}
