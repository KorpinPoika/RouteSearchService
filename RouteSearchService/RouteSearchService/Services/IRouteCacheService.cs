using RouteSearchService.Dtos.SearchService;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Services;

public interface IRouteCacheService
{
	void TryAddRoutes(IEnumerable<Route> routes);
	IEnumerable<Route> Find(SearchRequest sr);
}