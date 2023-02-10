using RouteSearchService.Dtos.One;
using RouteSearchService.Dtos.SearchService;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Extensions.ProviderOne;

public static class DtoExtensions
{
	public static ProviderOneSearchRequest ToProviderOneRequest(this SearchRequest sr) => new() {
		From = sr.Origin,
		To = sr.Destination,
		DateTo = sr.OriginDateTime
	};

	public static Route[] ToRoutes(this IEnumerable<ProviderOneRoute> routes) => routes.Select(r => r.ToRoute()).ToArray();
	
	private static Route ToRoute(this ProviderOneRoute r) => new() {
		Id = Guid.NewGuid(),	//todo: check it later
		Origin = r.From,
		Destination = r.To,
		OriginDateTime = r.DateFrom,
		DestinationDateTime = r.DateTo,
		Price = r.Price,
		TimeLimit = r.TimeLimit
	};
	
}