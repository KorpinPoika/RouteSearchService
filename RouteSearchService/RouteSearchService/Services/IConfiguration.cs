namespace RouteSearchService.Services;

public interface IConfiguration
{
	string RouteProviderOneUrl { get; }
	string RouteProviderTwoUrl { get; }
    
	int PingTimeout  { get; }
	int SearchTimeOut  { get; }
}