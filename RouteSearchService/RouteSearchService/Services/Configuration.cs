namespace RouteSearchService.Services;

public class Configuration : IConfiguration
{
	public string RouteProviderOneUrl => Config.RouteProviderOneUrl;
	public string RouteProviderTwoUrl => Config.RouteProviderTwoUrl;
	public int PingTimeout => Config.PingTimeout;
	public int SearchTimeOut => Config.SearchTimeOut;
}