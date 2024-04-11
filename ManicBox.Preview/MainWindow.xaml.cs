using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Interop;
using ManicBox.Interop;
using ReactiveUI;
using Point = System.Windows.Point;

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

			var windowFocus = User32
				.GetForegroundWindow( RxApp.MainThreadScheduler )
				.Where( window => window != thisWindow.Handle )
				.Publish();

			windowFocus
				.Select( window => User32.GetWindowTitle( window, RxApp.MainThreadScheduler ) )
				.Switch()
				.BindTo( this, view => view.TextBlock.Text )
				.DisposeWith( d );

			windowFocus
				.Select( window => Observable.Create<Thumbnail>( observer =>
					{
						var onDispose = new CompositeDisposable();

						var thumbnail = new Thumbnail( thisWindow.Handle, window )
							.SetOpacity( 255 )
							.SetVisible( true )
							.SetSourceClientAreaOnly( true );

						Observable.FromEventPattern(
								handler => this.LayoutUpdated += handler,
								handler => this.LayoutUpdated -= handler )
							.Select( _ => GetClientArea() )
							.StartWith( GetClientArea() )
							.Subscribe( rect => thumbnail.SetDestinationRect( rect ) )
							.DisposeWith( onDispose );

						return onDispose;
					} )
					.DisposeEach() )
				.Switch()
				.Subscribe()
				.DisposeWith( d );

			windowFocus
				.Connect()
				.DisposeWith( d );
		} );
	}

	private Rectangle GetClientArea()
	{
		var pos = this.ClientArea.TransformToAncestor( this )
			.Transform( new Point( 0, 0 ) );
		var size = this.ClientArea.RenderSize;
		return new Rectangle( (int) pos.X, (int) pos.Y, (int) size.Width, (int) size.Height );
	}
}