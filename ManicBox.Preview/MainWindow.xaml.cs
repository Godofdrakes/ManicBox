using System.Reactive.Linq;
using System.Windows.Interop;
using ManicBox.Interop;
using ReactiveUI;

namespace ManicBox.Preview;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private readonly IDisposable _disposable;

	public MainWindow()
	{
		InitializeComponent();

		_disposable = Observable
			.Interval( TimeSpan.FromSeconds( 5 ) )
			.Select( _ => User32.GetForegroundWindow() )
			.DistinctUntilChanged()
			.ObserveOn( RxApp.MainThreadScheduler )
			.Select( window =>
			{
				var thisWindow = new WindowInteropHelper( this );

				if ( window == thisWindow.Handle )
				{
					return null;
				}

				if ( window == nint.Zero )
				{
					return null;
				}

				return new Thumbnail( thisWindow.Handle, window )
					.SetOpacity( 255 )
					.SetVisible( true )
					.SetDestinationRect( ClientArea.GetRect() )
					.SetSourceClientAreaOnly( true );
			} )
			.DisposeEach()
			.Subscribe();
	}

	protected override void OnClosed( EventArgs e )
	{
		_disposable.Dispose();

		base.OnClosed( e );
	}
}