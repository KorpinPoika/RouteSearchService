namespace RouteProvider.Dtos.Two;

public class ProviderTwoSearchRequest
{
	// Mandatory
	// Start point of route, e.g. Moscow 
	public string Departure { get; set; } = null!;

	// Mandatory
	// End point of route, e.g. Sochi
	public string Arrival { get; set; } = null!;

	// Mandatory
	// Start date of route
	public DateTime DepartureDate { get; set; }
    
	// Optional
	// Minimum value of time limit for route
	public DateTime? MinTimeLimit { get; set; }
}