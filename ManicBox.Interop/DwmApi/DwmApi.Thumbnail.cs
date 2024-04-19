using System.Drawing;
using System.Runtime.InteropServices;
using ManicBox.Interop.Common;

namespace ManicBox.Interop;

public static partial class DwmApi
{
	public delegate void ThumbnailPropertyAction( ref ThumbnailProperties properties );

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
		internal Margins Destination;
		internal Margins Source;
		internal byte Opacity;
		[MarshalAs( UnmanagedType.Bool )] internal bool Visible;
		[MarshalAs( UnmanagedType.Bool )] internal bool SourceClientAreaOnly;

		public void SetDestinationRect( Margins rect )
		{
			Flags |= ThumbnailFlags.RectDestination;
			Destination = rect;
		}

		public void SetSourceRect( Margins rect )
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

	public sealed class Thumbnail : IDisposable
	{
		private HANDLE _handle;

		public Thumbnail( HWND windowDestination, HWND windowSource )
		{
			RegisterThumbnail( windowDestination, windowSource, out _handle );
		}

		public Thumbnail SetProperties( ref ThumbnailProperties thumbnailProperties )
		{
			if ( _handle.IsValid )
			{
				try
				{
					UpdateThumbnailProperties( _handle, ref thumbnailProperties );
				}
				catch ( ArgumentException )
				{
					// The thumbnail handle was already invalid
					_handle = HANDLE.Null;
				}
			}

			return this;
		}

		public Thumbnail SetProperties( ThumbnailPropertyAction action )
		{
			ArgumentNullException.ThrowIfNull( action );

			if ( _handle.IsValid )
			{
				var properties = new ThumbnailProperties();

				action( ref properties );

				try
				{
					UpdateThumbnailProperties( _handle, ref properties );
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