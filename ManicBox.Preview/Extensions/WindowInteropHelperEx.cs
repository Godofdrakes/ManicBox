using System.Windows;
using System.Windows.Interop;
using ManicBox.Interop;

namespace ManicBox.Preview.Extensions;

public static class WindowEx
{
	public static HWND GetHWND( this Window window )
	{
		ArgumentNullException.ThrowIfNull( window );

		return new HWND( new WindowInteropHelper( window ).Handle );
	}
}