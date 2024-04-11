using System.Reactive.Disposables;

namespace ManicBox.Interop.Extensions;

internal static class DisposableEx
{
	public static IDisposable AsDisposable<T>( this T userObject, Action<T> onDispose )
	{
		ArgumentNullException.ThrowIfNull( userObject );
		ArgumentNullException.ThrowIfNull( onDispose );

		return Disposable.Create( () => onDispose( userObject ) );
	}

	public static T DisposeWith<T>( this T disposable, CompositeDisposable compositeDisposable )
		where T : IDisposable
	{
		ArgumentNullException.ThrowIfNull( disposable );
		ArgumentNullException.ThrowIfNull( compositeDisposable );

		compositeDisposable.Add( disposable );

		return disposable;
	}
}