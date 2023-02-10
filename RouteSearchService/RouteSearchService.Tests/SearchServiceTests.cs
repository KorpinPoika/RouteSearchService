using Microsoft.Extensions.Logging;
using Moq;
using RouteSearchService.Services;

namespace RouteSearchService.Tests;

[TestFixture]
public class SearchServiceTests
{
	private readonly ILogger<SearchService> _logger = new Mock<ILogger<SearchService>>().Object;
	
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	[Category("Ping")]
	[Description("Ping should be true if even one of providers return true")]
	public async Task PositivePingTest()
	{
		var provider1 = new Mock<IRouteProvider>();
		var provider2 = new Mock<IRouteProvider>();

		provider1.Setup(x => x.PingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
		provider2.Setup(x => x.PingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

		var searchService = new SearchService(_logger, It.IsAny<IRouteCacheService>(), new[] {provider1.Object, provider2.Object});

		var result = await searchService.IsAvailableAsync(It.IsAny<CancellationToken>());
		
		Assert.That(result, Is.True);
	}

	[Test]
	[Category("Ping")]
	[Description("Ping should be false if all of providers return false")]
	public async Task NegativePingTest()
	{
		var provider1 = new Mock<IRouteProvider>();
		var provider2 = new Mock<IRouteProvider>();

		provider1.Setup(x => x.PingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
		provider2.Setup(x => x.PingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

		var searchService = new SearchService(_logger, It.IsAny<IRouteCacheService>(), new[] {provider1.Object, provider2.Object});

		var result = await searchService.IsAvailableAsync(It.IsAny<CancellationToken>());
		
		Assert.That(result, Is.False);
	}

}