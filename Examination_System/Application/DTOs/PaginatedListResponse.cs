namespace Application.DTOs
{
    public class PaginatedListResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PerPage);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        public PaginatedListResponse() { }

        public PaginatedListResponse(IEnumerable<T> data, int count, int page, int perPage)
        {
            Data = data;
            TotalCount = count;
            Page = page;
            PerPage = perPage;
        }
    }
}
