using System.Reactive.Disposables;
using ManicBox.Interop;
using ManicBox.Interop.Common;
using ManicBox.Services.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Services.ViewModel;

public sealed class WindowHandleViewModel : ReactiveObject, IDisposable
{
	[Reactive] public HWND Handle { get; set; }
	[Reactive] public string ProcessName { get; set; } = string.Empty;
	[Reactive] public string WindowTitle { get; set; } = string.Empty;
	[Reactive] public Margins WindowBounds { get; set; }
	[Reactive] public bool IsForegroundWindow { get; set; }

	private readonly CompositeDisposable _onDispose = new();

	public WindowHandleViewModel()
	{
		// Does nothing. Used for design-time data and mockups.
	}

	public WindowHandleViewModel( HWND hWnd, IWindowHandleService service )
	{
		ArgumentNullException.ThrowIfNull( service );

		if ( hWnd.IsNull )
		{
			throw new ArgumentNullException( nameof(hWnd) );
		}

		this.Handle = hWnd;

		service.OnTitleChange( hWnd )
			.BindTo( this, viewModel => viewModel.WindowTitle )
			.DisposeWith( _onDispose );

		service.OnMoveSize( hWnd )
			.BindTo( this, viewModel => viewModel.WindowBounds )
			.DisposeWith( _onDispose );

		service.IsForeground( hWnd )
			.BindTo( this, viewModel => viewModel.IsForegroundWindow )
			.DisposeWith( _onDispose );
	}

	public DwmApi.Thumbnail CreateThumbnail( HWND destinationWindow )
	{
		if ( _onDispose.IsDisposed )
		{
			throw new InvalidOperationException();
		}

		return new DwmApi.Thumbnail( destinationWindow, Handle )
			.DisposeWith( _onDispose );
	}

	public void Dispose()
	{
		_onDispose.Dispose();
	}
}