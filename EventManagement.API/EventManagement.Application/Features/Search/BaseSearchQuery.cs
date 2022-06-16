namespace EventManagement.Application.Features.Search
{
    public abstract class BaseSearchQuery
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 25;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value is > MaxPageSize or 0 ? MaxPageSize : value;
        }
    }
}
