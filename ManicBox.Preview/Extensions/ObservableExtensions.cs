using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ManicBox.Preview.Extensions;

public static class ObservableExtensions
{
	public static IObservable<T?> DisposeEach<T>( this IObservable<T?> observable )
		where T : IDisposable
	{
		return Observable.Using( () => new SerialDisposable(),
			serial => observable.Do( item => serial.Disposable = item ) );
	}

	public static IObservable<T> Publish<T>( this IObservable<T> observable, out IConnectableObservable<T> connectable )
	{
		ArgumentNullException.ThrowIfNull( observable );

		connectable = observable.Publish();

		return connectable;
	}
}