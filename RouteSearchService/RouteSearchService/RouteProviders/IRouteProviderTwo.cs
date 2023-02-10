using Microsoft.AspNetCore.Mvc;
using Refit;
using RouteSearchService.Dtos.Two;

namespace RouteSearchService.RouteProviders;

public interface IRouteProviderTwo: IRouteProviderBase
{
	[Post("/search")]
	Task<ProviderTwoSearchResponse> SearchAsync([FromBody] ProviderTwoSearchRequest request, CancellationToken cancellationToken);
}