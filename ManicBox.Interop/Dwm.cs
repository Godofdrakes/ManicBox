using System.Drawing;
using System.Runtime.InteropServices;

// ReSharper disable StringLiteralTypo

namespace ManicBox.Interop;

public static class Dwm
{
	[DllImport( "dwmapi.dll", EntryPoint = "DwmQueryThumbnailSourceSize" )]
	internal static extern int QueryThumbnailSourceSize( nint thumbnailHandle, out Size size );

	[DllImport( "dwmapi.dll", EntryPoint = "DwmRegisterThumbnail" )]
	internal static extern int RegisterThumbnail( nint windowDestination, nint windowSource, out nint thumbnailHandle );

	[DllImport( "dwmapi.dll", EntryPoint = "DwmUnregisterThumbnail" )]
	internal static extern int UnregisterThumbnail( nint thumbnailHandle );

	[DllImport( "dwmapi.dll", EntryPoint = "DwmUpdateThumbnailProperties" )]
	internal static extern int UpdateThumbnailProperties( nint thumbnailHandle, ref ThumbnailProperties thumbnailProperties );
}