using Refit;
using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Extensions.ProviderTwo;
using RouteSearchService.RouteProviders;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Services;

public class RouteProviderTwo : RouteProviderBase
{
	public RouteProviderTwo(ILogger<RouteProviderTwo> logger) : base(logger)
	{
	}

	protected override string RuteProviderUrl => Config.RouteProviderTwoUrl;
	
	protected override async Task<ICollection<Route>> SearchInternalAsync(SearchRequest request,
		CancellationToken cancellationToken)
	{
		var providerOneClient = RestService.For<IRouteProviderTwo>(Config.RouteProviderTwoUrl);
		var response = await providerOneClient.SearchAsync(request.ToProviderTwoRequest(), cancellationToken);

		return response.Routes.ToRoutes();
	}
}