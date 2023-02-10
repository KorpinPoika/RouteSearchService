using Refit;
using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Extensions.ProviderOne;
using RouteSearchService.RouteProviders;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Services;

public class RouteProviderOne : RouteProviderBase
{
	public RouteProviderOne(ILogger<RouteProviderOne> logger) : base(logger)
	{
	}

	protected override string RuteProviderUrl => Config.RouteProviderOneUrl;
	
	protected override async Task<ICollection<Route>> SearchInternalAsync(SearchRequest request,
		CancellationToken cancellationToken)
	{
		var providerOneClient = RestService.For<IRouteProviderOne>(Config.RouteProviderOneUrl);
		var response = await providerOneClient.SearchAsync(request.ToProviderOneRequest(), cancellationToken);

		return response.Routes.ToRoutes();
	}
}