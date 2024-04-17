using System.Drawing;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

public static partial class DwmApi
{
	public delegate void ThumbnailPropertyAction( ThumbnailPropertyBuilder builder );

	[Flags]
	internal enum ThumbnailFlags
	{
		// Rectangle destination of thumbnail
		RectDestination = 0x0000001,

		// Rectangle source of thumbnail
		RectSource = 0x0000002,

		// Opacity
		Opacity = 0x0000004,

		// Visibility
		Visible = 0x0000008,

		// Displays only the client area of the source
		SourceClientAreaOnly = 0x00000010
	}

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

	public sealed class Thumbnail : IDisposable
	{
		private HANDLE _handle;

		public Thumbnail( HWND windowDestination, HWND windowSource )
		{
			RegisterThumbnail( windowDestination, windowSource, out _handle );
		}

		public Size GetSourceSize()
		{
			if ( _handle.IsValid )
			{
				try
				{
					return QueryThumbnailSourceSize( _handle );
				}
				catch ( ArgumentException )
				{
					// The thumbnail handle was already invalid
					_handle = HANDLE.Null;
				}
			}

			return Size.Empty;
		}

		public Thumbnail SetProperties( ThumbnailPropertyAction action )
		{
			ArgumentNullException.ThrowIfNull( action );

			var builder = new ThumbnailPropertyBuilder();

			action( builder );

			if ( _handle.IsValid )
			{
				try
				{
					UpdateThumbnailProperties( _handle, ref builder.Properties );
				}
				catch ( ArgumentException )
				{
					// The thumbnail handle was already invalid
					_handle = HANDLE.Null;
				}
			}

			return this;
		}

		public void Dispose()
		{
			// @todo: thread safety?

			GC.SuppressFinalize( this );

			if ( _handle.IsValid )
			{
				try
				{
					DwmApi.UnregisterThumbnail( _handle );
				}
				catch ( ArgumentException )
				{
					// The thumbnail handle was already invalidated
				}
				finally
				{
					_handle = HANDLE.Null;
				}
			}
		}

		~Thumbnail() => Dispose();
	}
}