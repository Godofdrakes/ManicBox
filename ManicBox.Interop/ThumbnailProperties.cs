using System.Drawing;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

[StructLayout( LayoutKind.Sequential )]
public ref struct ThumbnailProperties
{
	internal ThumbnailFlags Flags;
	internal Rectangle Destination;
	internal Rectangle Source;
	internal byte Opacity;
	[MarshalAs( UnmanagedType.Bool )] internal bool Visible;
	[MarshalAs( UnmanagedType.Bool )] internal bool SourceClientAreaOnly;
}

public static class ThumbnailPropertiesEx
{
	public static ref ThumbnailProperties SetDestinationRect( this ref ThumbnailProperties properties, Rectangle rect )
	{
		properties.Flags |= ThumbnailFlags.RectDestination;
		properties.Destination = rect;

		return ref properties;
	}

	public static ref ThumbnailProperties SetSourceRect( this ref ThumbnailProperties properties, Rectangle rect )
	{
		properties.Flags |= ThumbnailFlags.RectSource;
		properties.Source = rect;

		return ref properties;
	}

	public static ref ThumbnailProperties SetOpacity( this ref ThumbnailProperties properties, byte opacity )
	{
		properties.Flags |= ThumbnailFlags.Opacity;
		properties.Opacity = opacity;

		return ref properties;
	}

	public static ref ThumbnailProperties SetVisible( this ref ThumbnailProperties properties, bool visible )
	{
		properties.Flags |= ThumbnailFlags.Visible;
		properties.Visible = visible;

		return ref properties;
	}

	public static ref ThumbnailProperties SetSourceClientAreaOnly( this ref ThumbnailProperties properties,
		bool sourceClientAreaOnly )
	{
		properties.Flags |= ThumbnailFlags.SourceClientAreaOnly;
		properties.SourceClientAreaOnly = sourceClientAreaOnly;

		return ref properties;
	}
}