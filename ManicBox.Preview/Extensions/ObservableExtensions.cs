using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ManicBox.Preview.Extensions;

public static class ObservableExtensions
{
	public static IObservable<T?> DisposeEach<T>( this IObservable<T?> observable )
		where T : IDisposable
	{
		return Observable.Using( () => new SerialDisposable(),
			serial => observable.Do( item => serial.Disposable = item ) );
	}
}