using System.Runtime.InteropServices;
using System.Text;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static partial class User32
{
	[DllImport( nameof(User32), SetLastError = false )]
	internal static extern nint GetShellWindow();

	[DllImport( nameof(User32), SetLastError = false )]
	internal static extern nint GetDesktopWindow();

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern nint GetForegroundWindow();

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern int GetWindowTextLength( nint hWnd );

	[DllImport( nameof(User32), SetLastError = true, CharSet = CharSet.Auto )]
	internal static extern int GetWindowText( nint hWnd, StringBuilder text, int count );

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern uint GetWindowThreadProcessId( nint hWnd, out uint lpdwProcessId );

	internal static string GetWindowTitle( nint handle )
	{
		var len = GetWindowTextLength( handle );

		if ( len < 1 )
		{
			MarshalUtil.ThrowLastError();

			return string.Empty;
		}

		var builder = new StringBuilder( len + 1 );

		if ( GetWindowText( handle, builder, len + 1 ) < 1 )
		{
			MarshalUtil.ThrowLastError();

			return string.Empty;
		}

		return builder.ToString();
	}

}