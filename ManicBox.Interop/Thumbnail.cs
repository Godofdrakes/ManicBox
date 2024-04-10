using System.Drawing;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public sealed class Thumbnail : IDisposable
{
	private ThumbnailProperties _properties;

	private nint _handle;

	public Thumbnail( nint windowDestination, nint windowSource )
	{
		HResult.ThrowIfError( Dwm.RegisterThumbnail(
			windowDestination,
			windowSource,
			out _handle ) );
	}

	private void UpdateThumbnailProperties()
	{
		try
		{
			HResult.ThrowIfError( Dwm.UpdateThumbnailProperties( _handle, ref _properties ) );
		}
		catch (ArgumentException)
		{
			// The thumbnail handle was already invalid
			_handle = nint.Zero;
		}
	}

	public Size GetSourceSize()
	{
		var size = new Size();

		if ( _handle != nint.Zero )
		{
			try
			{
				HResult.ThrowIfError( Dwm.QueryThumbnailSourceSize( _handle, out size ) );
			}
			catch (ArgumentException)
			{
				// The thumbnail handle was already invalid
				_handle = nint.Zero;
			}
		}

		return size;
	}

	public Thumbnail SetDestinationRect( Rectangle rect )
	{
		if ( _handle != nint.Zero )
		{
			_properties.Flags = ThumbnailFlags.RectDestination;
			_properties.Destination = rect;

			UpdateThumbnailProperties();
		}

		return this;
	}

	public Thumbnail SetSourceRect( Rectangle rect )
	{
		if ( _handle != nint.Zero )
		{
			_properties.Flags = ThumbnailFlags.RectSource;
			_properties.Source = rect;

			UpdateThumbnailProperties();
		}

		return this;
	}

	public Thumbnail SetOpacity( byte opacity )
	{
		if ( _handle != nint.Zero )
		{
			_properties.Flags = ThumbnailFlags.Opacity;
			_properties.Opacity = opacity;

			UpdateThumbnailProperties();
		}

		return this;
	}

	public Thumbnail SetVisible( bool visible )
	{
		if ( _handle != nint.Zero )
		{
			_properties.Flags = ThumbnailFlags.Visible;
			_properties.Visible = visible;

			UpdateThumbnailProperties();
		}

		return this;
	}

	public Thumbnail SetSourceClientAreaOnly( bool sourceClientAreaOnly )
	{
		if ( _handle != nint.Zero )
		{
			_properties.Flags = ThumbnailFlags.SourceClientAreaOnly;
			_properties.SourceClientAreaOnly = sourceClientAreaOnly;

			UpdateThumbnailProperties();
		}

		return this;
	}

	public void Dispose()
	{
		// @todo: thread safety?

		if ( _handle != nint.Zero )
		{
			try
			{
				HResult.ThrowIfError( Dwm.UnregisterThumbnail( _handle ) );
			}
			catch (ArgumentException)
			{
				// The thumbnail handle was already invalid
			}

			_handle = nint.Zero;
		}

		GC.SuppressFinalize( this );
	}

	~Thumbnail() => Dispose();
}