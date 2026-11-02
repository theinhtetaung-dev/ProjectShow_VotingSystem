namespace VotingSystem.Shared;

public sealed class PageResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public static class PagingExtensions
{
    public static Result<PageResult<T>> ToPageResult<T>(
        this Result<List<T>> source,
        int pageNumber,
        int pageSize)
    {
        if (!source.IsSuccess)
        {
            return source.GetEnumRespType() switch
            {
                Result<List<T>>.EnumRespType.ValidationError => Result<PageResult<T>>.ValidationError(source.Message),
                Result<List<T>>.EnumRespType.NotFound => Result<PageResult<T>>.NotFound(source.Message),
                _ => Result<PageResult<T>>.SystemError(source.Message)
            };
        }

        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var allItems = source.Data ?? [];
        var totalCount = allItems.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        pageNumber = Math.Min(pageNumber, totalPages);

        return Result<PageResult<T>>.Success(new PageResult<T>
        {
            Items = allItems.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        }, source.Message);
    }
}
