using Microsoft.AspNetCore.Mvc;
using Refit;
using RouteSearchService.Dtos.One;

namespace RouteSearchService.RouteProviders;

public interface IRouteProviderOne: IRouteProviderBase
{
	[Post("/search")]
	Task<ProviderOneSearchResponse> SearchAsync([FromBody] ProviderOneSearchRequest request, CancellationToken cancellationToken);
}