namespace CoreBanking.API.Models
{
    public class PaginationRequest(int pageSize = 10, int pageIndex = 0)
    {
        public int PageSize { get; set; } = pageSize;
        public int PageIndex { get; set; } = pageIndex;
    }
}
