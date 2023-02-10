namespace RouteSearchService.Dtos.SearchService;

public class SearchRequest
{
	// Mandatory
	// Start point of route, e.g. Moscow 
	public string Origin { get; set; } = null!;

	// Mandatory
	// End point of route, e.g. Sochi
	public string Destination { get; set; } = null!;

	// Mandatory
	// Start date of route
	public DateTime OriginDateTime { get; set; }
    
	// Optional
	public SearchFilters? Filters { get; set; }
}