using System.Net;
using Microsoft.AspNetCore.Mvc;
using Refit;
using RouteSearchService.Dtos.SearchService;
using RouteSearchService.RouteProviders;
using RouteSearchService.Services;

namespace RouteSearchService.Controllers;

[ApiController]
[Route("routes-searcher/api/v{version:apiVersion}/")]
[ApiVersion("1.0")]
public class RouteSearchController : ControllerBase
{
	private readonly ISearchService _searchService;
	private readonly ILogger<RouteSearchController> _logger;

	public RouteSearchController(ILogger<RouteSearchController> logger, ISearchService searchService)
	{
		_logger = logger;
		_searchService = searchService;
	}

	[MapToApiVersion("1.0")]
	[HttpPost("search")]
	public async Task<ActionResult<SearchResponse>> SearchAsync([FromBody] SearchRequest request)
	{
		try
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Config.SearchTimeOut));
			var response = await _searchService.SearchAsync(request, cts.Token);
	
			return Ok(response);
		}
		catch (Exception ex)
		{
			_logger.LogError($"Searching failed: {ex}");
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	[MapToApiVersion("1.0")]
	[HttpGet("ping")]
	public async Task<ActionResult<bool>> PingAsync()
	{
		try
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Config.PingTimeout));
			var result = await _searchService.IsAvailableAsync(cts.Token);

			return Ok(result);
		}
		catch (Exception ex)
		{
			_logger.LogError($"Ping Error: {ex}");
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}