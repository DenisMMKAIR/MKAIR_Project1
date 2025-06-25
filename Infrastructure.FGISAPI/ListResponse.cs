namespace Infrastructure.FGISAPI;

public class ListResponse<T>
{
    public required ListResult Result { get; set; }

    public class ListResult
    {
        public int Count { get; set; }
        public int Start { get; set; }
        public int Rows { get; set; }
        public required List<T> Items { get; set; }
    }
}