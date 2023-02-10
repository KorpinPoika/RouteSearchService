namespace RouteSearchService.Dtos.Two;

public class ProviderTwoRoute
{
	// Mandatory
	// Start point of route
	public ProviderTwoPoint Departure { get; set; } = null!;


	// Mandatory
	// End point of route
	public ProviderTwoPoint Arrival { get; set; } = null!;

	// Mandatory
	// Price of route
	public decimal Price { get; set; }
    
	// Mandatory
	// Timelimit. After it expires, route became not actual
	public DateTime TimeLimit { get; set; }
}