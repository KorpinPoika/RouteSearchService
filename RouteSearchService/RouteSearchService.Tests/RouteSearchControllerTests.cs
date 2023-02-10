using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RouteSearchService.Controllers;
using RouteSearchService.Services;

namespace RouteSearchService.Tests;

[TestFixture]
public class RouteSearchControllerTests
{
	private readonly ILogger<RouteSearchController> _logger = new Mock<ILogger<RouteSearchController>>().Object;
	private Mock<IConfiguration> _config;
	
	[SetUp]
	public void Setup()
	{
		_config = new Mock<IConfiguration>();
		_config.Setup(x => x.PingTimeout).Returns(5);
		_config.Setup(x => x.SearchTimeOut).Returns(5);
	}
	
	
	[Test]
	[Category("Ping")]
	[Description("Ping response should be OK and value should be same as search service returns")]
	public async Task PositivePingTest()
    {
        var searchService = new Mock<ISearchService>();
		searchService.Setup(x => x.IsAvailableAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

		var controller = new RouteSearchController(_logger, _config.Object, searchService.Object);

		var result = await controller.PingAsync();
		var responseResult = result.Result as OkObjectResult; 
		
		Assert.That(responseResult?.StatusCode, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(responseResult!.Value, Is.True);
        });
    }

	[Test]
	[Category("Ping")]
	[Description("Ping response should be 500 when search service produces an exception")]
	public async Task PingExceptionToleranceTest()
    {
        var searchService = new Mock<ISearchService>();
		searchService.Setup(
			x => x.IsAvailableAsync(It.IsAny<CancellationToken>())
		)
		.ThrowsAsync(new HttpRequestException());

		var controller = new RouteSearchController(_logger, _config.Object, searchService.Object);

		var result = await controller.PingAsync();
		var responseResult = result.Result as StatusCodeResult; 
		
		Assert.That(responseResult?.StatusCode, Is.Not.Null);
		Assert.That(responseResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
}

