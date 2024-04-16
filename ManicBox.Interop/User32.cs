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
		public nint HWind { get; }
		public uint IdThread { get; }
		public uint IdProcess { get; }

		public WindowReference( nint hWind )
		{
			this.HWind = hWind;
			this.IdThread = GetWindowThreadProcessId( hWind, out var idProcess );
			this.IdProcess = idProcess;
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

		if ( len < 1 )
		{
			return string.Empty;
		}

		var builder = new StringBuilder( len + 1 );

		if ( GetWindowText( handle, builder, len + 1 ) < 1 )
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
	public static IObservable<nint> ForegroundWindowChanged()
	{
		var shellThread = GetWindowThreadProcessId( GetShellWindow(), out var shellProcess );

		if ( shellThread == 0 )
		{
			MarshalUtil.ThrowLastError();
		}

		return Observable.Create<nint>( observer => EventHookBuilder
				.OnEvent( WinEvent.SystemForeground )
				.WithCallback( ( _, _, hWnd, _, _, _, _ ) => observer.OnNext( hWnd ) )
				.Subscribe() )
			.StartWith( GetForegroundWindow() )
			.Select( window => new WindowReference( window ) )
			.Where( window => window.IdProcess != shellProcess )
			.Select( window => window.HWind );
	}

	// Observe changes in a window's title
	public static IObservable<string> WindowTitleChanged( nint window )
	{
		var idThread = GetWindowThreadProcessId( window, out var idProcess );

		if ( idThread == 0 )
		{
			MarshalUtil.ThrowLastError();
		}

		return Observable.Create<nint>( observer => EventHookBuilder
				.OnEvent( WinEvent.ObjectNameChange )
				.ForProcess( idProcess, idThread )
				.WithCallback( ( _, _, w, _, _, _, _ ) => observer.OnNext( w ) )
				.Subscribe() )
			// We only care about changes to the window itself
			.Where( w => w == window )
			.StartWith( window )
			.Select( GetWindowTitle );
	}
}