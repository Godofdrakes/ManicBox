using System.Reactive.Disposables;
using System.Reactive.Linq;
using ManicBox.Interop;
using ManicBox.Preview.Extensions;
using ManicBox.Preview.ViewModel;
using ReactiveUI;

namespace ManicBox.Preview;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private ThumbnailViewModel Thumbnail { get; } = new();

	private WindowListViewModel WindowList { get; } = new();

	public MainWindow()
	{
		InitializeComponent();

		ThumbnailView.ViewModel = Thumbnail;
		WindowListView.ViewModel = WindowList;

		this.WhenActivated( d =>
		{
			var thisWindow = this.GetHWND();

			this.Thumbnail.DestinationWindow = thisWindow;

			// Observe the currently focused window
			var windowFocus = User32
				.OnForegroundWindowChanged()
				.Where( window => window != thisWindow )
				.SubscribeOn( RxApp.MainThreadScheduler )
				.Publish();

			windowFocus
				.BindTo( this, view => view.Thumbnail.SourceWindow )
				.DisposeWith( d );

			windowFocus
				.Connect()
				.DisposeWith( d );
		} );
	}
}