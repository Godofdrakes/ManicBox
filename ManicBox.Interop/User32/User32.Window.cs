using System.Runtime.InteropServices;
using System.Text;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static partial class User32
{
	public delegate bool EnumWindowsProc( HWND hWnd, nint lParam );
	
	[DllImport( nameof(User32), SetLastError = false )]
	internal static extern int GetWindowLong( HWND hWnd, int nIndex );

	[DllImport( nameof(User32), SetLastError = false )]
	internal static extern HWND GetShellWindow();

	[DllImport( nameof(User32), SetLastError = false )]
	internal static extern HWND GetDesktopWindow();

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern HWND GetForegroundWindow();

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern int GetWindowTextLength( HWND hWnd );

	[DllImport( nameof(User32), SetLastError = true, CharSet = CharSet.Auto )]
	internal static extern int GetWindowText( HWND hWnd, StringBuilder text, int count );

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern uint GetWindowThreadProcessId( HWND hWnd, out uint lpdwProcessId );

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern bool EnumWindows( EnumWindowsProc lpEnumFunc, nint lParam );

	internal static string GetWindowTitle( HWND hWnd )
	{
		var len = GetWindowTextLength( hWnd );

		if ( len < 1 )
		{
			MarshalUtil.ThrowLastError();

			return string.Empty;
		}

		var builder = new StringBuilder( len + 1 );

		if ( GetWindowText( hWnd, builder, len + 1 ) < 1 )
		{
			MarshalUtil.ThrowLastError();

			return string.Empty;
		}

		return builder.ToString();
	}

	internal static IEnumerable<HWND> EnumerateWindows()
	{
		var windows = new List<HWND>();

		bool EnumWindowsProc( HWND hWnd, IntPtr _ )
		{
			windows.Add( hWnd );
			return true;
		}

		if ( !EnumWindows( EnumWindowsProc, default ) )
		{
			MarshalUtil.ThrowLastError();
		}

		return windows;
	}

	internal static IEnumerable<HWND> EnumerateWindows( Func<HWND, bool> filter )
	{
		ArgumentNullException.ThrowIfNull( filter );

		var windows = new List<HWND>();

		bool EnumWindowsProc( HWND hWnd, IntPtr _ )
		{
			if ( filter( hWnd ) )
			{
				windows.Add( hWnd );
			}

			return true;
		}

		if ( !EnumWindows( EnumWindowsProc, default ) )
		{
			MarshalUtil.ThrowLastError();
		}

		return windows;
	}
}