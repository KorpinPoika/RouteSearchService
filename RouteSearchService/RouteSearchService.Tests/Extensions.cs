using System.Globalization;

namespace RouteSearchService.Tests;

public static class Extensions
{
	public static DateTime ToDateTime(this string dt) =>
		DateTime.ParseExact(dt, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
}