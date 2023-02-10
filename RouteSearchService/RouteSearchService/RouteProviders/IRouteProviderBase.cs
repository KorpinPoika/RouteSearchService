using Refit;

namespace RouteSearchService.RouteProviders;

public interface IRouteProviderBase
{
	[Get("/ping")]
	Task PingAsync(CancellationToken cancellationToken);
}

