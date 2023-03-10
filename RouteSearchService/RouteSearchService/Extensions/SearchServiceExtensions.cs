using RouteSearchService.Dtos.SearchService;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Extensions;

public static class SearchServiceExtensions
{
	public static SearchResponse Empty(this SearchResponse sr)
	{
		sr.Routes = Enumerable.Empty<Route>().ToArray();
		return sr;
	}

	public static bool IsEmpty(this SearchResponse? sr) => sr?.Routes.Any() != true;

	public static SearchResponse CalculateStatistic(this SearchResponse sr)
	{
		if (sr.Routes.Any())
		{
			var routesDuration = sr.Routes.Select(r => r.DestinationDateTime.Subtract(r.OriginDateTime)).ToArray();

			sr.MinPrice = sr.Routes.Select(r => r.Price).Min();
			sr.MaxPrice = sr.Routes.Select(r => r.Price).Max();
			sr.MinMinutesRoute = Convert.ToInt32(routesDuration.Min().TotalMinutes);
			sr.MaxMinutesRoute = Convert.ToInt32(routesDuration.Max().TotalMinutes);
		}

		return sr;
	}

	public static SearchResponse Merge(this SearchResponse dest, SearchResponse src)
	{
		if (dest.Routes.Any() || src.Routes.Any())
		{
			var destKeys = dest.Routes.Select(r => r.GetKey()).ToList();
			var routesList = dest.Routes.ToList();
			
			routesList.AddRange(src.Routes.Where(r => !destKeys.Contains(r.GetKey())));
		
			dest.Routes = routesList.ToArray();
			dest.MinPrice = (src.MinPrice < dest.MinPrice) ? src.MinPrice : dest.MinPrice;
			dest.MaxPrice = (src.MaxPrice > dest.MaxPrice) ? src.MaxPrice : dest.MaxPrice;
			dest.MinMinutesRoute = (src.MinMinutesRoute < dest.MinMinutesRoute) ? src.MinMinutesRoute : dest.MinMinutesRoute;
			dest.MaxMinutesRoute = (src.MaxMinutesRoute > dest.MaxMinutesRoute) ? src.MaxMinutesRoute : dest.MaxMinutesRoute;
		}
		
		return dest;
	}

	public static SearchResponse Filter(this SearchResponse sr, SearchFilters filters) => new SearchResponse {
		Routes = sr.Routes.Where(
			r => filters.DestinationDateTime?.With(ddt => r.DestinationDateTime <= ddt) ?? true	
		)
		.Where(
			r => filters.MaxPrice?.With(mp => r.Price <= mp) ?? true	
		)	
		.Where(
			r => filters.MinTimeLimit?.With(mtl => r.TimeLimit <= mtl) ?? true	
		)	
		.ToArray()
	}
	.CalculateStatistic();


	public static string GetKey(this Route r) => $"{r.Origin}-{r.Destination}-{r.OriginDateTime}-{r.DestinationDateTime}-{r.Price}-{r.TimeLimit}";
}