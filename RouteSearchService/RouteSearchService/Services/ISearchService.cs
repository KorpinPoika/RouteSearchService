using RouteSearchService.Dtos.SearchService;

namespace RouteSearchService.Services;

public interface ISearchService
{
	Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
	Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}