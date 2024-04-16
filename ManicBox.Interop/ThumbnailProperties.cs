using System.Drawing;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

[StructLayout( LayoutKind.Sequential )]
public struct ThumbnailProperties
{
	internal ThumbnailFlags Flags;
	internal Rectangle Destination;
	internal Rectangle Source;
	internal byte Opacity;
	[MarshalAs( UnmanagedType.Bool )] internal bool Visible;
	[MarshalAs( UnmanagedType.Bool )] internal bool SourceClientAreaOnly;
}

public class ThumbnailPropertyBuilder
{
	internal ThumbnailProperties Properties;

	public ThumbnailPropertyBuilder SetDestinationRect( Rectangle rect )
	{
		Properties.Flags |= ThumbnailFlags.RectDestination;
		Properties.Destination = rect;

		return this;
	}

	public ThumbnailPropertyBuilder SetSourceRect( Rectangle rect )
	{
		Properties.Flags |= ThumbnailFlags.RectSource;
		Properties.Source = rect;

		return this;
	}

	public ThumbnailPropertyBuilder SetOpacity( byte opacity )
	{
		Properties.Flags |= ThumbnailFlags.Opacity;
		Properties.Opacity = opacity;

		return this;
	}

	public ThumbnailPropertyBuilder SetVisible( bool visible )
	{
		Properties.Flags |= ThumbnailFlags.Visible;
		Properties.Visible = visible;

		return this;
	}

	public ThumbnailPropertyBuilder SetSourceClientAreaOnly( bool sourceClientAreaOnly )
	{
		Properties.Flags |= ThumbnailFlags.SourceClientAreaOnly;
		Properties.SourceClientAreaOnly = sourceClientAreaOnly;

		return this;
	}
}