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
					this.ClientArea.Width = size.Width;
					this.ClientArea.Height = size.Height;
				} )
				.DisposeWith( d );

			this.OnLayoutUpdated()
				.Select( _ => GetClientArea() )
				.BindTo( ViewModel, viewModel => viewModel.DestinationRect )
				.DisposeWith( d );
		} );
	}

	private Margins GetClientArea()
	{
		var window = Window.GetWindow( this );

		var topLeft = this.ClientArea
			.TransformToAncestor( window! )
			.Transform( new Point( 0, 0 ) );

		var bottomRight = this.ClientArea
			.TransformToAncestor( window! )
			.Transform( new Point( this.ClientArea.ActualWidth, this.ClientArea.ActualHeight ) );

		return new Margins( (int)topLeft.X, (int)topLeft.Y, (int)bottomRight.X, (int)bottomRight.Y );
	}
}