using System.Net;
using Refit;
using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Extensions;
using Route = RouteSearchService.Dtos.SearchService.Route;

namespace RouteSearchService.Services;

public abstract class RouteProviderBase : IRouteProvider
{
	private const string pingAction = "ping";

	private readonly ILogger _logger;

	protected RouteProviderBase(ILogger logger)
	{
		_logger = logger;
	}

	public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var routes = await SearchInternalAsync(request, cancellationToken);

			return new SearchResponse { Routes = routes.ToArray() }.CalculateStatistic();
		}
		catch (ApiException ex)
		{
			_logger.LogError($"Searching routes failed: {ex}");
			return new SearchResponse().Empty();
		}
		catch (Exception ex)
		{
			_logger.LogError($"Searching routes failed: {ex}");
			throw;
		}
	}

	public async Task<bool> PingAsync(CancellationToken cancellationToken)
	{
		try
		{
			using var httpClient = new HttpClient();
			var response = await httpClient.GetAsync($"{RuteProviderUrl}{pingAction}", cancellationToken);

			return response.StatusCode == HttpStatusCode.OK;
		}
		catch (TaskCanceledException ex)
		{
			_logger.LogInformation($"{(ex.CancellationToken.IsCancellationRequested? "Ping operation is cancelled" : "Ping timeout occured")}");
			return false;
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError($"{RuteProviderUrl} ping is unavailable: {ex.Message}");
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError($"{RuteProviderUrl} ping failed: {ex.Message}");
			throw;
		}
	}
	
	protected abstract string RuteProviderUrl { get; }

	protected abstract Task<ICollection<Route>> SearchInternalAsync(SearchRequest request,
		CancellationToken cancellationToken);
}