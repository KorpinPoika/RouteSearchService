namespace RouteSearchService.Dtos.One;

public class ProviderOneRoute
{
	// Mandatory
	// Start point of route
	public string From { get; set; } = null!;
    
	// Mandatory
	// End point of route
	public string To { get; set; } = null!;
    
	// Mandatory
	// Start date of route
	public DateTime DateFrom { get; set; }
    
	// Mandatory
	// End date of route
	public DateTime DateTo { get; set; }
    
	// Mandatory
	// Price of route
	public decimal Price { get; set; }
    
	// Mandatory
	// Time limit. After it expires, route became not actual
	public DateTime TimeLimit { get; set; }
}