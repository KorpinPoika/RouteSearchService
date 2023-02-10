using RouteSearchService.Dtos.SearchService;

namespace RouteSearchService.Services;

public interface IRouteProvider
{
	Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
	Task<bool> PingAsync(CancellationToken cancellationToken);
}