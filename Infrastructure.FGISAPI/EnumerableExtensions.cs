namespace Infrastructure.FGISAPI;

internal static class EnumerableExtensions
{
    public static IEnumerable<IReadOnlyList<T>> SplitBy<T>(this IEnumerable<T> source, uint chunkSize)
    {
        if (chunkSize == 0)
        {
            throw new ArgumentException("Chunk size must be greater than zero.", nameof(chunkSize));
        }

        using var enumerator = source.GetEnumerator();
        while (true)
        {
            var chunk = new List<T>((int)chunkSize);
            for (uint i = 0; i < chunkSize && enumerator.MoveNext(); i++)
            {
                chunk.Add(enumerator.Current);
            }

            if (chunk.Count == 0)
            {
                break;
            }

            yield return chunk;
        }
    }
}
