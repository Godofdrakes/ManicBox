using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.Interop;
using ManicBox.Interop.Common;
using ManicBox.Reactive.Services.Interface;
using ManicBox.Reactive.ViewModel;
using ReactiveUI;

namespace ManicBox.Reactive.Services.Implementation;

public sealed class WindowHandleService : IWindowHandleService
{
	private readonly IObservable<HWND> _windowMoveSize;
	private readonly IObservable<HWND> _windowTitleChange;
	private readonly IObservable<HWND> _windowFocusChange;

	private readonly IObservable<IChangeSet<WindowHandleViewModel, HWND>> _windowViewModels;

	public WindowHandleService()
	{
		_windowMoveSize = User32.OnWindowMoveSize()
			.SubscribeOn( RxApp.MainThreadScheduler )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Publish()
			.RefCount();

		_windowTitleChange = User32.OnWindowTitleChanged()
			.SubscribeOn( RxApp.MainThreadScheduler )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Publish()
			.RefCount();

		_windowFocusChange = User32.OnForegroundWindowChanged()
			.SubscribeOn( RxApp.MainThreadScheduler )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Publish()
			.RefCount();

		_windowViewModels = ObservableChangeSet.Create<HWND, HWND>( cache =>
				{
					var onDispose = new CompositeDisposable();

					User32.OnWindowCreated()
						.SubscribeOn( RxApp.MainThreadScheduler )
						.ObserveOn( RxApp.MainThreadScheduler )
						.Subscribe( cache.AddOrUpdate )
						.DisposeWith( onDispose );

					User32.OnWindowDestroyed()
						.SubscribeOn( RxApp.MainThreadScheduler )
						.ObserveOn( RxApp.MainThreadScheduler )
						.Subscribe( cache.RemoveKey )
						.DisposeWith( onDispose );

					foreach ( HWND hwnd in User32.EnumerateWindows() )
					{
						cache.AddOrUpdate( hwnd );
					}

					return onDispose;
				},
				hwnd => hwnd )
			.Transform( hWnd => new WindowHandleViewModel( hWnd, this ) )
			.DisposeMany()
			.SubscribeOn( RxApp.MainThreadScheduler )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Publish()
			.RefCount();
	}

	public IObservable<IChangeSet<WindowHandleViewModel, HWND>> EnumerateWindows()
	{
		return _windowViewModels;
	}

	public IObservable<Margins> OnMoveSize( HWND hWnd )
	{
		return _windowMoveSize
			.StartWith( hWnd )
			.Where( window => window == hWnd )
			.Select( User32.GetWindowRect );
	}

	public IObservable<string> OnTitleChange( HWND hWnd )
	{
		return _windowTitleChange
			.StartWith( hWnd )
			.Where( window => window == hWnd )
			.Select( User32.GetWindowTitle );
	}

	public IObservable<bool> IsForeground( HWND hWnd )
	{
		return _windowFocusChange.Select( window => window == hWnd );
	}
}