using System.Runtime.InteropServices;
using System.Text;

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
}