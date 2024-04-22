using System.Reactive.Disposables;
using System.Reactive.Linq;
using ManicBox.Interop;
using ManicBox.Interop.Common;
using ManicBox.Reactive.Services.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Reactive.ViewModel;

public sealed class WindowHandleViewModel : ReactiveObject, IDisposable
{
	[Reactive] public string ProcessName { get; private set; } = string.Empty;
	[Reactive] public string WindowTitle { get; private set; } = string.Empty;
	[Reactive] public Margins WindowBounds { get; private set; }
	[Reactive] public bool IsForegroundWindow { get; private set; }

	private readonly CompositeDisposable _onDispose = new();

	private readonly HWND _hWnd;

	public WindowHandleViewModel( HWND hWnd, IWindowHandleService windowHandleService )
	{
		if ( hWnd.IsNull )
		{
			throw new ArgumentNullException( nameof(hWnd) );
		}

		ArgumentNullException.ThrowIfNull( windowHandleService );

		_hWnd = hWnd;

		windowHandleService.OnTitleChange( hWnd )
			.BindTo( this, viewModel => viewModel.WindowTitle )
			.DisposeWith( _onDispose );

		windowHandleService.OnMoveSize( hWnd )
			.BindTo( this, viewModel => viewModel.WindowBounds )
			.DisposeWith( _onDispose );

		windowHandleService.IsForeground( hWnd )
			.BindTo( this, viewModel => viewModel.IsForegroundWindow )
			.DisposeWith( _onDispose );
	}

	public DwmApi.Thumbnail CreateThumbnail( HWND destinationWindow )
	{
		if ( _onDispose.IsDisposed )
		{
			throw new InvalidOperationException();
		}

		return new DwmApi.Thumbnail( destinationWindow, _hWnd )
			.DisposeWith( _onDispose );
	}

	public void Dispose()
	{
		_onDispose.Dispose();
	}
}