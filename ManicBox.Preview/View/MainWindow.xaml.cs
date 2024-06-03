using System.Reactive.Disposables;
using ManicBox.Preview.Extensions;
using ReactiveUI;

namespace ManicBox.Preview.View;

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
			this.WhenAnyValue( view => view.ViewModel )
				.WhereNotNull()
				.Subscribe( viewModel => viewModel.OwningWindowHandle = this.GetHWND() )
				.DisposeWith( d );

			this.OneWayBind( ViewModel,
					viewModel => viewModel.WindowListViewModel,
					view => view.WindowListView.ViewModel )
				.DisposeWith( d );

			this.OneWayBind( ViewModel,
					viewModel => viewModel.ThumbnailViewModel,
					view => view.ThumbnailView.ViewModel )
				.DisposeWith( d );

			this.OneWayBind( ViewModel,
				viewModel => viewModel.WindowListViewModel.Windows.Count,
				view => view.ItemListStatus.Text,
				count => $"{count} Items" );
		} );
	}
}