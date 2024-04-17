using System.Drawing;
using System.Runtime.InteropServices;

// ReSharper disable StringLiteralTypo

namespace ManicBox.Interop;

public static partial class DwmApi
{
	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmQueryThumbnailSourceSize" )]
	internal static extern Size QueryThumbnailSourceSize( HANDLE thumbnailHandle );

	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmRegisterThumbnail" )]
	internal static extern void RegisterThumbnail( HWND windowDestination, HWND windowSource, out HANDLE thumbnailHandle );

	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmUnregisterThumbnail" )]
	internal static extern void UnregisterThumbnail( HANDLE thumbnailHandle );

	[DllImport( nameof(DwmApi), PreserveSig = false, EntryPoint = "DwmUpdateThumbnailProperties" )]
	internal static extern void UpdateThumbnailProperties( HANDLE thumbnailHandle, ref ThumbnailProperties thumbnailProperties );
}