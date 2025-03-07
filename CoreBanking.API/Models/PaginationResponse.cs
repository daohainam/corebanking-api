namespace CoreBanking.API.Models;
public class PaginationResponse<TEntity>(int index, int pageSize, long count, IEnumerable<TEntity> items)
{
    public int Index => index;
    public int PageSize => pageSize;
    public long TotalCount => count;
    public IEnumerable<TEntity> Items => items;
}
