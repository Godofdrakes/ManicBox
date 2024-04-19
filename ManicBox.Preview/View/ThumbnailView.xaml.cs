using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ManicBox.Interop.Common;
using ManicBox.Reactive.Extensions;
using ReactiveUI;
using Size = System.Windows.Size;

namespace ManicBox.Preview.View;

public partial class ThumbnailView
{
	public ThumbnailView()
	{
		InitializeComponent();

		this.WhenActivated( d =>
		{
			this.WhenAnyValue( view => view.ViewModel!.HasThumbnail )
				.Select( b => b ? Visibility.Collapsed : Visibility.Visible )
				.BindTo( this, view => view.TextBlock.Visibility )
				.DisposeWith( d );

			this.WhenAnyValue( view => view.ViewModel!.SourceSize )
				.Select( size => new Size( size.Width, size.Height ) )
				.Subscribe( size =>
				{
					ClientArea.Width = size.Width;
					ClientArea.Height = size.Height;
				} )
				.DisposeWith( d );

			this.OnLayoutUpdated()
				.Select( _ => GetClientArea() )
				.BindTo( ViewModel, viewModel => viewModel.DestinationRect )
				.DisposeWith( d );

			this.OnLayoutUpdated()
				.Select( _ => IsUserVisible() )
				.BindTo( ViewModel, viewModel => viewModel.Visible )
				.DisposeWith( d );
		} );
	}

	private bool IsUserVisible()
	{
		if ( !IsVisible )
		{
			return false;
		}

		var window = Window.GetWindow( this );

		if ( window is null )
		{
			return false;
		}

		var windowBounds = new Rect( 0, 0, window.ActualWidth, window.ActualHeight );
		var clientBounds = new Rect( 0, 0, ClientArea.ActualWidth, ClientArea.ActualHeight );

		var finalBounds = ClientArea
			.TransformToAncestor( window )
			.TransformBounds( clientBounds );

		return windowBounds.IntersectsWith( finalBounds );
	}

	private Margins GetClientArea()
	{
		if ( !IsVisible )
		{
			return default;
		}

		var window = Window.GetWindow( this );

		if ( window is null )
		{
			return default;
		}

		var clientBounds = new Rect( 0, 0, ClientArea.ActualWidth, ClientArea.ActualHeight );

		var finalBounds = ClientArea
			.TransformToAncestor( window )
			.TransformBounds( clientBounds );

		return new Margins(
			(int)finalBounds.Left,
			(int)finalBounds.Top,
			(int)finalBounds.Right,
			(int)finalBounds.Bottom );
	}
}