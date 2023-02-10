using Microsoft.AspNetCore.Mvc;
using RouteProvider.Dtos.One;

namespace RouteProvider.Controllers;

[ApiController]
[Route("provider-one/api/v{version:apiVersion}/")]
[ApiVersion("1.0")]
public class ProviderOneController : PingableController
{
	private readonly ILogger<ProviderOneController> _logger;

	public ProviderOneController(ILogger<ProviderOneController> logger)
	{
		_logger = logger;
	}

	[MapToApiVersion("1.0")]
	[HttpGet("ping")]
	public override Task<IActionResult> Ping()
	{
		return base.Ping();
	}

	[MapToApiVersion("1.0")]
	[HttpPost("search")]
	public async Task<ActionResult<ProviderOneSearchResponse>> Search([FromBody] ProviderOneSearchRequest request)
	{
		try
		{
			await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(5)));

			return Ok(
				new ProviderOneSearchResponse {
					Routes = new [] {
						new ProviderOneRoute{
							From = "Moscow",
							To = "Adler",
							Price = 1000,
							DateFrom = DateTime.Parse("01.01.2023"),
							DateTo = DateTime.Parse("01.02.2023"),
							TimeLimit = DateTime.Parse("01.03.2023")
						}
					}
				}
			);
		}
		catch (Exception ex)
		{
			_logger.LogError($"Searching failed with: {ex}");
			return StatusCode(StatusCodes.Status500InternalServerError, $"Searching failed - {ex.Message}");
		}
	}
}