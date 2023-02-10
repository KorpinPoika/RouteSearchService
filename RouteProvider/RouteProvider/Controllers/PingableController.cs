using Microsoft.AspNetCore.Mvc;

namespace RouteProvider.Controllers;


public class PingableController: ControllerBase
{
	public virtual async Task<IActionResult> Ping()
	{
		await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next((5))));
		
		if (Random.Shared.Next(100) < 50)
		{
			return Ok();
		}

		return StatusCode(StatusCodes.Status500InternalServerError);
	}
}