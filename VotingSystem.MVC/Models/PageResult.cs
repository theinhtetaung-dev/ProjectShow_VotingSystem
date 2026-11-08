namespace VotingSystem.MVC.Models;

public sealed class PageResult<T>
{
    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; } = 1;
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public int Count => Items.Count;
    public T this[int index] => Items[index];
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
}
