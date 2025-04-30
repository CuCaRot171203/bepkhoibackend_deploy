namespace BepKhoiBackend.Shared.Helpers
{
    public class PagedResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
