using System.Drawing;

namespace ManicBox.Interop;

public sealed class Thumbnail : IDisposable
{
	public delegate void ThumbnailPropertyAction( ThumbnailPropertyBuilder builder );

	private nint _handle;

	public Thumbnail( nint windowDestination, nint windowSource )
	{
		_handle = DwmApi.RegisterThumbnail( windowDestination, windowSource );
	}

	public Size GetSourceSize()
	{
		if ( _handle != nint.Zero )
		{
			try
			{
				return DwmApi.QueryThumbnailSourceSize( _handle );
			}
			catch ( ArgumentException )
			{
				// The thumbnail handle was already invalid
				_handle = nint.Zero;
			}
		}

		return Size.Empty;
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
				DwmApi.UpdateThumbnailProperties( _handle, ref builder.Properties );
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
				DwmApi.UnregisterThumbnail( _handle );
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