using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Dtos.Two;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Extensions.ProviderTwo;

public static class DtoExtensions
{
	public static ProviderTwoSearchRequest ToProviderTwoRequest(this SearchRequest sr) => new() {
		Departure = sr.Origin,
		Arrival = sr.Destination,
		DepartureDate = sr.OriginDateTime,
		MinTimeLimit = sr.Filters?.MinTimeLimit
	};
	
	public static Route[] ToRoutes(this IEnumerable<ProviderTwoRoute> routes) => routes.Select(r => r.ToRoute()).ToArray();

	private static Route ToRoute(this ProviderTwoRoute r) => new() {
		Id = Guid.NewGuid(),	//todo: check it later
		Origin = r.Departure.Point,
		Destination = r.Arrival.Point,
		OriginDateTime = r.Departure.Date,
		DestinationDateTime = r.Arrival.Date,
		Price = r.Price,
		TimeLimit = r.TimeLimit
	};
	
}