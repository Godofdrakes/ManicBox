using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.Interop;
using ManicBox.Interop.Common;
using ManicBox.Services.Interface;
using ManicBox.Services.ViewModel;
using ReactiveUI;

namespace ManicBox.Services.Implementation;

public sealed class WindowMonitorService : IWindowMonitorService
{
	private readonly IObservable<HWND> _windowMoveSize;
	private readonly IObservable<HWND> _windowTitleChange;
	private readonly IObservable<HWND> _windowFocusChange;

	private readonly IObservable<IChangeSet<WindowHandleViewModel, HWND>> _windowViewModels;

	public WindowMonitorService()
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
			.Transform( hWnd =>
			{
				var viewModel = new WindowHandleViewModel()
				{
					Handle = hWnd
				};

				viewModel.WithDisposables( onDispose =>
				{
					this.OnTitleChange( viewModel.Handle )
						.BindTo( viewModel, vm => vm.WindowTitle )
						.DisposeWith( onDispose );

					this.OnMoveSize( viewModel.Handle )
						.BindTo( viewModel, vm => vm.WindowBounds )
						.DisposeWith( onDispose );

					this.IsForeground( viewModel.Handle )
						.BindTo( viewModel, vm => vm.IsForegroundWindow )
						.DisposeWith( onDispose );
				} );

				return viewModel;
			} )
			.DisposeMany()
			.SubscribeOn( RxApp.MainThreadScheduler )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Publish()
			.RefCount();
	}

	public IObservable<IChangeSet<WindowHandleViewModel, HWND>> GetWindows()
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