using System.Drawing;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

[StructLayout( LayoutKind.Sequential )]
public ref struct ThumbnailProperties
{
	private ThumbnailFlags Flags;
	private Rectangle Destination;
	private Rectangle Source;
	private byte Opacity;
	[MarshalAs( UnmanagedType.Bool )] private bool Visible;
	[MarshalAs( UnmanagedType.Bool )] private bool SourceClientAreaOnly;

	public void SetDestinationRect( Rectangle rect )
	{
		Flags |= ThumbnailFlags.RectDestination;
		Destination = rect;
	}

	public void SetSourceRect( Rectangle rect )
	{
		Flags |= ThumbnailFlags.RectSource;
		Source = rect;
	}

	public void SetOpacity( byte opacity )
	{
		Flags |= ThumbnailFlags.Opacity;
		Opacity = opacity;
	}

	public void SetVisible( bool visible )
	{
		Flags |= ThumbnailFlags.Visible;
		Visible = visible;
	}

	public void SetSourceClientAreaOnly( bool sourceClientAreaOnly )
	{
		Flags |= ThumbnailFlags.SourceClientAreaOnly;
		SourceClientAreaOnly = sourceClientAreaOnly;
	}
}