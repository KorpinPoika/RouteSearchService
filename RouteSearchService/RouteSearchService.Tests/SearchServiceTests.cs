using System.Globalization;
using Microsoft.Extensions.Logging;
using Moq;
using RouteSearchService.Dtos.SearchService;
using RouteSearchService.Services;

namespace RouteSearchService.Tests;

[TestFixture]
public class SearchServiceTests
{
	private readonly ILogger<SearchService> _logger = new Mock<ILogger<SearchService>>().Object;
	private readonly Mock<IRouteCacheService> _cacheMock = new Mock<IRouteCacheService>();
	private ICollection<Route> _testRoutesListOne;
	private ICollection<Route> _testRoutesListTwo;

	[SetUp]
	public void Setup()
	{
		_cacheMock.Setup(x => x.TryAddRoutes(It.IsAny<IEnumerable<Route>>()));
		_cacheMock.Setup(x => x.Find(It.IsAny<SearchRequest>())).Returns(Enumerable.Empty<Route>());
		
		_testRoutesListOne = new List<Route> {
			new() {
				Id = Guid.NewGuid(),
				Origin = "Moscow",
				OriginDateTime = "15.01.2023 10:00".ToDateTime(),
				Destination = "Tbilisi",
				DestinationDateTime = "17.01.2023 15:00".ToDateTime(),
				Price = 50000,
				TimeLimit = "17.01.2023 15:00".ToDateTime(),
			},
			new() {
				Id = Guid.NewGuid(),
				Origin = "Novgorod",
				OriginDateTime = "21.02.2023 10:00".ToDateTime(),
				Destination = "Saint-Petersburg",
				DestinationDateTime = "21.02.2023 16:00".ToDateTime(),
				Price = 3000,
				TimeLimit = "21.02.2023 16:00".ToDateTime(),
			}
		};
		
		_testRoutesListTwo = new List<Route> {
			new() {
				Id = Guid.NewGuid(),
				Origin = "Oranienbaum",
				OriginDateTime = "18.01.2023 10:00".ToDateTime(),
				Destination = "Tihvin",
				DestinationDateTime = "18.01.2023 17:00".ToDateTime(),
				Price = 5000,
				TimeLimit = "18.01.2023 17:00".ToDateTime(),
			},
			new Route {
				Id = Guid.NewGuid(),
				Origin = "Kronshtadt",
				OriginDateTime = "25.02.2023 10:00".ToDateTime(),
				Destination = "Vyborg",
				DestinationDateTime = "25.02.2023 14:00".ToDateTime(),
				Price = 2500,
				TimeLimit = "25.02.2023 14:00".ToDateTime(),
			}
		};
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

	[Test]
	[Category("Search")]
	[Description("Search should find routes (without filtration)")]
	public async Task SearchRoutesWithoutFiltrationTest()
    {
        var provider1 = new Mock<IRouteProvider>();
		var provider2 = new Mock<IRouteProvider>();

		provider1.Setup(x => x.SearchAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
			new SearchResponse { Routes = _testRoutesListOne.ToArray() }	
		);
		provider2.Setup(x => x.SearchAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
			new SearchResponse { Routes = _testRoutesListTwo.ToArray() }	
		);
		
		var searchService = new SearchService(_logger, _cacheMock.Object, new[] {provider1.Object, provider2.Object});

		var result = await searchService.SearchAsync(
			new SearchRequest {
				Origin = "Moscow", Destination = "Tbilisi", OriginDateTime = "15.01.2023 10:00".ToDateTime() 
			},
			It.IsAny<CancellationToken>()
		);
		
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Routes, Is.Not.Empty);
		Assert.That(result.Routes, Has.Length.EqualTo(4));
        Assert.Multiple(() =>
        {
            Assert.That(result.Routes[0].Origin, Is.EqualTo("Moscow"));
            Assert.That(result.Routes[0].Destination, Is.EqualTo("Tbilisi"));
            Assert.That(result.Routes[0].OriginDateTime, Is.EqualTo("15.01.2023 10:00".ToDateTime()));
        });
    }

	[Test]
	[Category("Search")]
	[Description("Search should find routes by filter parameters")]
	public async Task SearchRoutesWithFiltrationTest()
    {
        var provider1 = new Mock<IRouteProvider>();
		var provider2 = new Mock<IRouteProvider>();

		provider1.Setup(x => x.SearchAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
			new SearchResponse { Routes = _testRoutesListOne.ToArray() }	
		);
		provider2.Setup(x => x.SearchAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
			new SearchResponse { Routes = _testRoutesListTwo.ToArray() }	
		);
		
		var searchService = new SearchService(_logger, _cacheMock.Object, new[] {provider1.Object, provider2.Object});

		var result = await searchService.SearchAsync(
			new SearchRequest {
				Origin = "Moscow", Destination = "Tbilisi", OriginDateTime = "15.01.2023 10:00".ToDateTime(),
				Filters = new SearchFilters {
					DestinationDateTime = "17.01.2023 15:00".ToDateTime(),
					MaxPrice = 100000,
					MinTimeLimit = "21.01.2023 15:00".ToDateTime(),
					OnlyCached = false
				}
			},
			It.IsAny<CancellationToken>()
		);
		
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Routes, Is.Not.Empty);
		Assert.That(result.Routes, Has.Length.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(result.Routes[0].Origin, Is.EqualTo("Moscow"));
            Assert.That(result.Routes[0].Destination, Is.EqualTo("Tbilisi"));
            Assert.That(result.Routes[0].OriginDateTime, Is.EqualTo("15.01.2023 10:00".ToDateTime()));
        });
    }

	[Test]
	[Category("Search")]
	[Description("Search should find routes only in cache by filter parameters")]
	public async Task SearchRoutesFromCacheWithFiltrationTest()
    {
	    var cache = new Mock<IRouteCacheService>();
	    cache.Setup(x => x.Find(It.IsAny<SearchRequest>())).Returns(_testRoutesListOne);
		
		var searchService = new SearchService(_logger, cache.Object, Enumerable.Empty<IRouteProvider>());

		var result = await searchService.SearchAsync(
			new SearchRequest {
				Origin = "Moscow", Destination = "Tbilisi", OriginDateTime = "15.01.2023 10:00".ToDateTime(),
				Filters = new SearchFilters {
					DestinationDateTime = "17.01.2023 15:00".ToDateTime(),
					MaxPrice = 100000,
					MinTimeLimit = "21.01.2023 15:00".ToDateTime(),
					OnlyCached = true
				}
			},
			It.IsAny<CancellationToken>()
		);
		
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Routes, Is.Not.Empty);
		Assert.That(result.Routes, Has.Length.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(result.Routes[0].Origin, Is.EqualTo("Moscow"));
            Assert.That(result.Routes[0].Destination, Is.EqualTo("Tbilisi"));
            Assert.That(result.Routes[0].OriginDateTime, Is.EqualTo("15.01.2023 10:00".ToDateTime()));
        });
    }
}