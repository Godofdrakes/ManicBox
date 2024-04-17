using System.Drawing;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using Point = System.Windows.Point;
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

			this.OnLayoutUpdated()
				.Select( _ => GetClientArea() )
				.BindTo( ViewModel, viewModel => viewModel.DestinationRect )
				.DisposeWith( d );
		} );
	}

	private IObservable<EventPattern<object>> OnLayoutUpdated()
	{
		return Observable.FromEventPattern(
			handler => this.LayoutUpdated += handler,
			handler => this.LayoutUpdated -= handler );
	}

	private Rectangle GetClientArea()
	{
		var window = Window.GetWindow( this );

		Point pos = this.ClientArea
			.TransformToAncestor( window! )
			.Transform( new Point( 0, 0 ) );

		Size size = this.ClientArea.RenderSize;

		return new Rectangle( (int)pos.X, (int)pos.Y, (int)size.Width, (int)size.Height );
	}
}