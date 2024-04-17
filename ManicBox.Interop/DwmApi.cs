using System.Drawing;
using System.Runtime.InteropServices;

// ReSharper disable StringLiteralTypo

namespace ManicBox.Interop;

public static class DwmApi
{
	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmQueryThumbnailSourceSize" )]
	internal static extern Size QueryThumbnailSourceSize( nint thumbnailHandle );

	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmRegisterThumbnail" )]
	internal static extern nint RegisterThumbnail( nint windowDestination, nint windowSource );

	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmUnregisterThumbnail" )]
	internal static extern void UnregisterThumbnail( nint thumbnailHandle );

	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmUpdateThumbnailProperties" )]
	internal static extern void UpdateThumbnailProperties( nint thumbnailHandle, ref ThumbnailProperties thumbnailProperties );
}