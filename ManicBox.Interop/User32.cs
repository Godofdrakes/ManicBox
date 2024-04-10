using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static class User32
{
	[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
	public static extern nint GetForegroundWindow();

	[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
	internal static extern int GetWindowText( nint hWnd, StringBuilder text, int count );

	[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
	internal static extern int GetWindowTextLength( nint hWnd );

	public static string GetWindowTitle( nint handle )
	{
		var len = GetWindowTextLength( handle );

		if ( len < 1 )
		{
			return string.Empty;
		}

		var builder = new StringBuilder( len );

		if ( GetWindowText( handle, builder, len ) < 1 )
		{
			return string.Empty;
		}

		return builder.ToString();
	}

	[DllImport( "user32.dll", SetLastError = true )]
	private static extern nint SetWinEventHook(
		int eventMin,
		int eventMax,
		nint hmodWinEventProc,
		WinEventProc lpfnWinEventProc,
		int idProcess,
		int idThread,
		int dwflags );

	[DllImport( "user32.dll", SetLastError = true )]
	private static extern int UnhookWinEvent( nint hWinEventHook );

	private delegate void WinEventProc(
		nint hWinEventHook,
		uint eventType,
		nint hwnd,
		int idObject,
		int idChild,
		uint dwEventThread,
		uint dwmsEventTime );

	private const int WINEVENT_INCONTEXT = 4;
	private const int WINEVENT_OUTOFCONTEXT = 0;
	private const int WINEVENT_SKIPOWNPROCESS = 2;
	private const int WINEVENT_SKIPOWNTHREAD = 1;

	private const int EVENT_SYSTEM_FOREGROUND = 3;

	public static IObservable<nint> OnForegroundWindowChanged( IScheduler scheduler )
	{
		return Observable.Create<nint>( observer =>
			{
				var eventHook = SetWinEventHook(
					EVENT_SYSTEM_FOREGROUND,
					EVENT_SYSTEM_FOREGROUND,
					nint.Zero,
					( _, _, window, _, _, _, _ ) => observer.OnNext( window ),
					0,
					0,
					WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS
				);

				if ( eventHook == nint.Zero )
				{
					HResult.ThrowLastPInvokeError();
				}

				return Disposable.Create( () => HResult.ThrowIfError( UnhookWinEvent( eventHook ) ) );
			} )
			.SubscribeOn( scheduler );
	}
}