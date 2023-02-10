namespace RouteSearchService;

public static class Config
{
    public static void Init(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            // This one needs to be last
            //.AddEnvironmentVariables();
        
        AppConfiguration = builder.Build();
    }

    public static IConfiguration AppConfiguration { get; private set; } = null!;

    public static string RouteProviderOneUrl => AppConfiguration.GetValue<string>("RouteProvider:One");
    public static string RouteProviderTwoUrl => AppConfiguration.GetValue<string>("RouteProvider:Two");
    
    public static int PingTimeout => AppConfiguration.GetValue<int>("OperationsTimeLimit:PingSec");
    public static int SearchTimeOut => AppConfiguration.GetValue<int>("OperationsTimeLimit:SearchSec");
}
