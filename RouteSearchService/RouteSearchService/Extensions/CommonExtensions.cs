namespace RouteSearchService.Extensions;

public static class CommonExtensions
{
	/// <summary>
	/// default(TResult) or action's result
	/// </summary>
	public static TResult With<TItem, TResult>(this TItem item, Func<TItem, TResult> action)
	{
		return (Equals(item, null)
			? default(TResult)
			: action(item))!;
	}

	/// <summary>
	/// Applies action on source and return the source.
	/// Fast path for constructions like this x => { source.SomeProperty = someValue; return source; }
	/// </summary>
	public static T Do<T>(this T source, Action<T> action)
	{
		if (!Equals(source, null))
		{
			action(source);
		}
		return source;
	}    }