using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Extensions;
using static System.String;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Services;

public class SearchService : ISearchService
{
	private readonly ILogger<SearchService> _logger;
	private readonly IRouteCacheService _cache;
	private readonly ICollection<IRouteProvider> _routeProviders;

	public SearchService(ILogger<SearchService> logger, IRouteCacheService cacheService, IEnumerable<IRouteProvider> routeProviders)
	{
		_logger = logger;
		_cache = cacheService;
		_routeProviders = routeProviders.ToList();
	}

	public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
	{
		var results = (request.Filters?.OnlyCached == true)
			? await SearchByCacheAsync(request)
			: await SearchByProvidersAsync(request, cancellationToken);

		var resultList = results.ToList();

		var result = (resultList.Count > 1) 
			? resultList.Select(
				r => r.CalculateStatistic()
			)
			.Aggregate((x, y) => x.Merge(y))
			: resultList.FirstOrDefault();

		if (result.IsEmpty()) {
			return new SearchResponse().Empty();
		}

		if (request.Filters?.OnlyCached != true) {
			AddRoutesInCacheAsync(result!.Routes);
		}
		
		return request.Filters?.With(f => result.Filter(f)) ?? result;
	}

	public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
	{
		var tcs = new TaskCompletionSource<bool>();
		
		var pingTasks = _routeProviders.Select(p => MakePingCallWithContinuation(p, tcs, cancellationToken)).ToList();
		await Task.WhenAny(tcs.Task, Task.WhenAll(pingTasks));

		if (tcs.Task.IsCompleted) {
			return true;
		}

		return false;
	}

	private Task MakePingCallWithContinuation(IRouteProvider p, TaskCompletionSource<bool> tcs, CancellationToken ct)
	{
		return p.PingAsync(ct).ContinueWith(
			r => {
				if (r.Result) {
					tcs.SetResult(true);
				}
			}
			, ct
		);
	}

	private async ValueTask<IEnumerable<SearchResponse>> SearchByProvidersAsync(SearchRequest request, CancellationToken cancellationToken)
	{
		var searchTasks = _routeProviders.Select(p => p.SearchAsync(request, cancellationToken)).ToList();
		return await Task.WhenAll(searchTasks);
	}

	private ValueTask<IEnumerable<SearchResponse>> SearchByCacheAsync(SearchRequest request) => ValueTask.FromResult(
		new List<SearchResponse> {
			new SearchResponse {
				Routes = _cache.Find(request).ToArray()
			}
			.CalculateStatistic()
		}
		.AsEnumerable()
	);

	private void AddRoutesInCacheAsync(IEnumerable<Route> routes) => _cache.TryAddRoutes(routes);
	
}