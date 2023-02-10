namespace RouteSearchService.Services;

public class CacheCleanerJob: BackgroundService
{
	private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(30));
	private readonly IRouteCacheService _cacheService;

	public CacheCleanerJob(IRouteCacheService cacheService)
	{
		_cacheService = cacheService;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
		{
			_cacheService.CleanExpiredItems();
		}
	}
}