using System.Reactive.Disposables;
using ReactiveUI;

namespace ManicBox.WPF.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MonitorWindow
{
	public MonitorWindow()
	{
		InitializeComponent();

		this.WhenActivated( d =>
		{
			this.OneWayBind( ViewModel,
					viewModel => viewModel.WindowMatchService,
					view => view.WindowMatchService.ViewModel )
				.DisposeWith( d );

			this.OneWayBind( ViewModel,
					viewModel => viewModel.WindowMonitorService,
					view => view.WindowMonitorService.ViewModel )
				.DisposeWith( d );
		} );
	}
}