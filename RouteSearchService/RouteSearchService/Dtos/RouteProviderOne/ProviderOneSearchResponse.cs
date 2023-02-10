namespace RouteSearchService.Dtos.One;

public class ProviderOneSearchResponse
{
	// Mandatory
	// Array of routes
	public ProviderOneRoute[] Routes { get; set; } = null!;
}