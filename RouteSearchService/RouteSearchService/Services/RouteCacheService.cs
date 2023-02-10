using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Extensions;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Services;

public class RouteCacheService: IRouteCacheService
{
	private readonly ReaderWriterLockSlim _lockSlim = new(LockRecursionPolicy.NoRecursion);
	private readonly ILogger<RouteCacheService> _logger;
	private readonly IDictionary<string, Route> _cache = new Dictionary<string, Route>();

	public RouteCacheService(ILogger<RouteCacheService> logger)
	{
		_logger = logger;
	}

	public void TryAddRoutes(IEnumerable<Route> routes)
	{
		if (!_lockSlim.TryEnterWriteLock(TimeSpan.FromSeconds(5))) {
			throw new InvalidOperationException("Cannot get the read-lock");
		}

		try
		{
			routes.ToList().ForEach(
				r =>
				{
					var key = r.GetKey();
					if (!_cache.ContainsKey(key)) {
						_cache[key] = r;
					}
				}	
			);
		}
		finally
		{
			_lockSlim.ExitWriteLock();
		}
	}

	public IEnumerable<Route> Find(SearchRequest sr)
	{
		if (!_lockSlim.TryEnterReadLock(TimeSpan.FromSeconds(5))) {
			throw new InvalidOperationException("Cannot get the read-lock");
		}

		try
		{
			return _cache.Select(
				x => x.Value	
			)
			.Where(r => r.Origin == sr.Origin && r.Destination == sr.Destination && r.OriginDateTime == sr.OriginDateTime)
			.ToList();

		}
		finally
		{
			_lockSlim.ExitReadLock();
		}
	}

	public void CleanExpiredItems()
	{
		try
		{
			List<string> toDelete;
			var now = DateTime.UtcNow;

			if (!_lockSlim.TryEnterReadLock(TimeSpan.FromSeconds(5))) {
				throw new InvalidOperationException("Cannot get the read-lock");
			}
			
			try
			{
				toDelete = _cache.Where(x => x.Value.TimeLimit <= now).Select(x => x.Key).ToList();
			}
			finally
			{
				_lockSlim.ExitReadLock();
			}

			if (!toDelete.Any()) {
				return;
			}
			
			if (!_lockSlim.TryEnterWriteLock(TimeSpan.FromSeconds(5))) {
				throw new InvalidOperationException("Cannot get the write-lock");
			}

			try
			{
				toDelete.ForEach(k => _cache.Remove(k));
			}
			finally
			{
				_lockSlim.ExitWriteLock();
			}

		}
		catch (Exception ex)
		{
			_logger.LogError($"Clear cache failed: {ex}");
		}
	}
}