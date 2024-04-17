using System.Reactive.Disposables;
using ReactiveUI;

namespace ManicBox.Preview.View;

public partial class WindowListView
{
	public WindowListView()
	{
		InitializeComponent();

		this.WhenActivated( d =>
		{
			this.OneWayBind( ViewModel, viewModel => viewModel.Windows, view => view.ListView.ItemsSource )
				.DisposeWith( d );
		} );
	}
}