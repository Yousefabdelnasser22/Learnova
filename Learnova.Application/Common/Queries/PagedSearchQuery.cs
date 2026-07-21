namespace Learnova.Application.Common.Queries
{
    public abstract class PagedSearchQuery : SearchQuery
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
