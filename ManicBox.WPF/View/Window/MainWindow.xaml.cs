using System.Reactive.Disposables;
using ReactiveUI;

namespace ManicBox.WPF.View;

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
			this.OneWayBind( ViewModel,
					viewModel => viewModel.ProcessFilterViewModel,
					view => view.FilterView.ViewModel )
				.DisposeWith( d );
		} );
	}
}