using System.Reactive.Linq;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static partial class User32
{
	// Observe changes in window focus
	public static IObservable<HWND> OnForegroundWindowChanged()
	{
		return EventHook( WinEvent.SystemForeground )
			.Select( ev => ev.hWnd )
			.StartWith( GetForegroundWindow() )
			.Where( IsShellWindow( false ) );
	}

	// Observe changes in a window's title
	public static IObservable<string> OnWindowTitleChanged( HWND hWnd )
	{
		var idThread = GetWindowThreadProcessId( hWnd, out var idProcess );

		if ( idThread == 0 )
		{
			MarshalUtil.ThrowLastError();
		}

		return EventHook( WinEvent.ObjectNameChange, idProcess, idThread )
			.Where( e => e.idObject == OBJID_WINDOW )
			.Where( e => e.idChild == CHILDID_SELF )
			.Select( e => e.hWnd )
			.StartWith( hWnd )
			.Select( GetWindowTitle );
	}

	private const int WS_VISIBLE = 0x10000000;
	private const int GWL_STYLE = -16;

	public static IObservable<HWND> OnWindowCreated()
	{
		var nowWindows = Observable.Create<HWND>( observer =>
			EnumerateWindows( hwnd =>
				{
					var style = GetWindowLong( hwnd, GWL_STYLE );
					var title = GetWindowTextLength( hwnd );
					return (style & WS_VISIBLE) != 0 && title > 0;
				} )
				.ToObservable()
				.Subscribe( observer ) );

		var newWindows = EventHook( WinEvent.ObjectCreate )
			.Where( e => e.idObject == OBJID_WINDOW )
			.Where( e => e.idObject == CHILDID_SELF )
			.Select( e => e.hWnd );

		return nowWindows.Concat( newWindows );
	}

	public static IObservable<HWND> OnWindowDestroyed()
	{
		return EventHook( WinEvent.ObjectDestroy )
			.Where( e => e.idObject == OBJID_WINDOW )
			.Where( e => e.idObject == CHILDID_SELF )
			.Select( e => e.hWnd );
	}

	private static Func<HWND, bool> IsShellWindow( bool expected )
	{
		var shellThread = GetWindowThreadProcessId( GetShellWindow(), out var shellProcess );

		if ( shellThread == 0 )
		{
			MarshalUtil.ThrowLastError();
		}

		return window =>
		{
			var windowThread = GetWindowThreadProcessId( window, out var windowProcess );

			if ( windowThread == 0 )
			{
				MarshalUtil.ThrowLastError();
			}

			return (windowProcess == shellProcess) == expected;
		};
	}
}