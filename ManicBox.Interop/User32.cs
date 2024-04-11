using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static class User32
{
	internal delegate void WinEventProc(
		nint hWinEventHook,
		uint eventType,
		nint hwnd,
		int idObject,
		int idChild,
		uint idEventThread,
		uint dwmsEventTime );

	internal struct WindowReference
	{
		public nint hWind { get; }
		public uint idThread { get; }
		public uint idProcess { get; }

		public WindowReference( nint hWind )
		{
			this.hWind = hWind;
			this.idThread = GetWindowThreadProcessId( hWind, out var idProcess );
			this.idProcess = idProcess;
		}
	}

	[DllImport( "user32.dll", SetLastError = false )]
	internal static extern nint GetShellWindow();

	[DllImport( "user32.dll", SetLastError = false )]
	internal static extern nint GetDesktopWindow();

	[DllImport( "user32.dll", SetLastError = true )]
	internal static extern nint GetForegroundWindow();

	[DllImport( "user32.dll", SetLastError = true )]
	internal static extern int GetWindowTextLength( nint hWnd );

	[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
	internal static extern int GetWindowText( nint hWnd, StringBuilder text, int count );

	[DllImport( "user32.dll", SetLastError = true )]
	internal static extern uint GetWindowThreadProcessId( nint hWnd, out uint lpdwProcessId );

	internal static string GetWindowTitle( nint handle )
	{
		var len = GetWindowTextLength( handle );

		if (len < 1)
		{
			return string.Empty;
		}

		var builder = new StringBuilder( len + 1 );

		if (GetWindowText( handle, builder, len + 1 ) < 1)
		{
			return string.Empty;
		}

		return builder.ToString();
	}

	[DllImport( "user32.dll", SetLastError = true )]
	internal static extern nint SetWinEventHook(
		WinEvent eventMin,
		WinEvent eventMax,
		nint hmodWinEventProc,
		WinEventProc lpfnWinEventProc,
		uint idProcess,
		uint idThread,
		WinEventFlags dwflags );

	[DllImport( "user32.dll", SetLastError = true )]
	internal static extern bool UnhookWinEvent( nint hWinEventHook );

	internal enum WinEvent : uint
	{
		SystemForeground = 0x0003,
		ObjectNameChange = 0x800C,
	}

	[Flags]
	internal enum WinEventFlags : uint
	{
		OutOfContext = 0,
		SkipOwnThread = 1,
		SkipOwnProcess = 2,
		InContext = 4,
	}

	// Observe changes in window focus
	public static IObservable<nint> GetForegroundWindow( IScheduler scheduler )
	{
		var shellThread = GetWindowThreadProcessId( GetShellWindow(), out var shellProcess );

		if (shellThread == 0)
		{
			HResult.ThrowLastPInvokeError();
		}

		return Observable.Create<nint>( observer => new WinEventHook( WinEvent.SystemForeground,
				( _, _, hWnd, _, _, _, _ ) => observer.OnNext( hWnd ) ) )
			.StartWith( GetForegroundWindow() )
			.Select( window => new WindowReference( window ) )
			.Where( window => window.idProcess != shellProcess )
			.Select( window => window.hWind )
			// Events are invoked on the subscribing thread.
			// Unsubscribe must occur on the subscribing thread.
			// Require scheduler to enforce this.
			.SubscribeOn( scheduler );
	}

	// Observe changes in a window's title
	public static IObservable<string> GetWindowTitle( nint window, IScheduler scheduler )
	{
		var idThread = GetWindowThreadProcessId( window, out var idProcess );

		if (idThread == 0)
		{
			HResult.ThrowLastPInvokeError();
		}

		return Observable.Create<nint>( observer => new WinEventHook(
				WinEvent.ObjectNameChange,
				idProcess,
				idThread,
				( _, _, w, _, _, _, _ ) => observer.OnNext( w ) ) )
			// We only care about changes to the window itself
			.Where( w => w == window )
			.StartWith( window )
			.Select( GetWindowTitle )
			// Events are invoked on the subscribing thread.
			// Unsubscribe must occur on the subscribing thread.
			// Require scheduler to enforce this.
			.SubscribeOn( scheduler );
	}
}