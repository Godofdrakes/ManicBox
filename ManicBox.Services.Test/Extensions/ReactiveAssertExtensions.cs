using System.Reactive.Linq;
using Microsoft.Reactive.Testing;

namespace ManicBox.Services.Test.Extensions;

internal static class ReactiveAssertExtensions
{
	public static void StartsWith<TSource>( this IObservable<TSource> observable, params TSource[] expected )
	{
		ReactiveAssert.AreElementsEqual( observable.Take( expected.Length ), expected.ToObservable() );
	}
}