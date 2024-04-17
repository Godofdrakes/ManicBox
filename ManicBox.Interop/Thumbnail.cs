using System.Drawing;

namespace ManicBox.Interop;

public sealed class Thumbnail : IDisposable
{
	public delegate void ThumbnailPropertyAction( ThumbnailPropertyBuilder builder );

	private HANDLE _handle;

	public Thumbnail( HWND windowDestination, HWND windowSource )
	{
		DwmApi.RegisterThumbnail( windowDestination, windowSource, out _handle );
	}

	public Size GetSourceSize()
	{
		if ( _handle.IsValid )
		{
			try
			{
				return DwmApi.QueryThumbnailSourceSize( _handle );
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
				DwmApi.UpdateThumbnailProperties( _handle, ref builder.Properties );
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