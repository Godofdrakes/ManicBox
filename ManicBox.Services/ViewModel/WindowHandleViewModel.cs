using System.Reactive.Disposables;
using ManicBox.Interop;
using ManicBox.Interop.Common;
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

	public DwmApi.Thumbnail CreateThumbnail( HWND destinationWindow )
	{
		if ( _onDispose.IsDisposed )
		{
			throw new InvalidOperationException();
		}

		return new DwmApi.Thumbnail( destinationWindow, Handle )
			.DisposeWith( _onDispose );
	}

	public void WithDisposables( Action<CompositeDisposable> action )
	{
		ArgumentNullException.ThrowIfNull( action );

		var onDispose = new CompositeDisposable();

		action( onDispose );

		_onDispose.Add( onDispose );
	}

	public void Dispose()
	{
		_onDispose.Dispose();
	}
}