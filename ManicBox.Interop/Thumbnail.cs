using System.Drawing;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

public sealed class Thumbnail : IDisposable
{
	public delegate void ThumbnailPropertyAction( ThumbnailPropertyBuilder builder );

	private nint _handle;

	public Thumbnail( nint windowDestination, nint windowSource )
	{
		Marshal.ThrowExceptionForHR( Dwm.RegisterThumbnail(
			windowDestination,
			windowSource,
			out _handle ) );
	}

	public Size GetSourceSize()
	{
		var size = new Size();

		if ( _handle != nint.Zero )
		{
			try
			{
				Marshal.ThrowExceptionForHR( Dwm.QueryThumbnailSourceSize( _handle, out size ) );
			}
			catch ( ArgumentException )
			{
				// The thumbnail handle was already invalid
				_handle = nint.Zero;
			}
		}

		return size;
	}

	public Thumbnail SetProperties( ThumbnailPropertyAction action )
	{
		ArgumentNullException.ThrowIfNull( action );

		var builder = new ThumbnailPropertyBuilder();

		action( builder );

		if ( _handle != nint.Zero )
		{
			try
			{
				Marshal.ThrowExceptionForHR( Dwm
					.UpdateThumbnailProperties( _handle, ref builder.Properties ) );
			}
			catch ( ArgumentException )
			{
				// The thumbnail handle was already invalid
				_handle = nint.Zero;
			}
		}

		return this;
	}

	public void Dispose()
	{
		// @todo: thread safety?

		GC.SuppressFinalize( this );

		if ( _handle != nint.Zero )
		{
			try
			{
				Marshal.ThrowExceptionForHR( Dwm.UnregisterThumbnail( _handle ) );
			}
			catch ( ArgumentException )
			{
				// The thumbnail handle was already invalidated
			}
			finally
			{
				_handle = nint.Zero;
			}
		}
	}

	~Thumbnail() => Dispose();
}