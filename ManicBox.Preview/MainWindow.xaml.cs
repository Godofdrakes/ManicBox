using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Interop;
using ManicBox.Interop;
using ReactiveUI;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace ManicBox.Preview;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public MainWindow()
	{
		InitializeComponent();

		this.WhenActivated( d =>
		{
			var thisWindow = new WindowInteropHelper( this );

			// Observe the currently focused window
			var windowFocus = User32
				.ForegroundWindowChanged()
				.SubscribeOn( RxApp.MainThreadScheduler )
				.Where( window => window != thisWindow.Handle )
				.Publish();

			// Observe the title of said window
			windowFocus
				.Select( User32.WindowTitleChanged )
				.SubscribeOn( RxApp.MainThreadScheduler )
				.Switch()
				.BindTo( this, view => view.TextBlock.Text )
				.DisposeWith( d );

			// Generate a thumbnail of said window
			var windowThumbnail = windowFocus
				.Select( window => new Thumbnail( thisWindow.Handle, window )
					.SetProperties( ( ref ThumbnailProperties props ) => props
						.SetOpacity( 255 )
						.SetVisible( true )
						.SetSourceClientAreaOnly( true )
						.SetDestinationRect( GetClientArea() ) ) )
				.DisposeEach()
				.Publish();

			// Resize said thumbnail
			Observable.FromEventPattern(
					handler => this.LayoutUpdated += handler,
					handler => this.LayoutUpdated -= handler )
				.Select( _ => GetClientArea() )
				.WithLatestFrom( windowThumbnail,
					( rectangle, thumbnail ) => (Rect: rectangle, Thumb: thumbnail) )
				.Subscribe( tuple => tuple.Thumb?
					.SetProperties( ( ref ThumbnailProperties props ) =>
						props.SetDestinationRect( tuple.Rect ) ) )
				.DisposeWith( d );

			windowThumbnail
				.Connect()
				.DisposeWith( d );

			windowFocus
				.Connect()
				.DisposeWith( d );
		} );
	}

	private Rectangle GetClientArea()
	{
		Point pos = this.ClientArea
			.TransformToAncestor( this )
			.Transform( new Point( 0, 0 ) );

		Size size = this.ClientArea.RenderSize;

		return new Rectangle( (int)pos.X, (int)pos.Y, (int)size.Width, (int)size.Height );
	}
}