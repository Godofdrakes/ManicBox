using System.Drawing;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

[StructLayout( LayoutKind.Sequential )]
internal struct ThumbnailProperties
{
	public ThumbnailFlags Flags;
	public Rectangle Destination;
	public Rectangle Source;
	public byte Opacity;
	[MarshalAs( UnmanagedType.Bool )] public bool Visible;
	[MarshalAs( UnmanagedType.Bool )] public bool SourceClientAreaOnly;
}