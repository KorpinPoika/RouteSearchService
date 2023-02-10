using Microsoft.AspNetCore.Mvc;
using RouteProvider.Dtos.Two;

namespace RouteProvider.Controllers;

[ApiController]
[Route("provider-two/api/v{version:apiVersion}/")]
[ApiVersion("1.0")]
public class ProviderTwoController: PingableController
{
	private readonly ILogger<ProviderTwoController> _logger;

	public ProviderTwoController(ILogger<ProviderTwoController> logger)
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
	public async Task<ActionResult<ProviderTwoSearchResponse>> Search([FromBody] ProviderTwoSearchRequest request)
	{
		try
		{
			await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(5)));

			return Ok(
				new ProviderTwoSearchResponse {
					Routes = new [] {
						new ProviderTwoRoute{
							Departure = new ProviderTwoPoint {
								Point = "Moscow",
								Date = DateTime.Parse("01.01.2023")
							},
							Arrival = new ProviderTwoPoint {
								Point = "Adler",
								Date = DateTime.Parse("01.02.2023")
							},
							Price = 1000,
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