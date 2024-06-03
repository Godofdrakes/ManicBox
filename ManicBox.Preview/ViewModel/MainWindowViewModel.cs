using System.Reactive.Disposables;
using ManicBox.Common.ViewModel;
using ManicBox.Interop;
using ManicBox.Reactive.Services.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Preview.ViewModel;

public class MainWindowViewModel : WindowViewModel
{
	[Reactive] public HWND OwningWindowHandle { get; set; }

	public WindowListViewModel WindowListViewModel { get; }

	public ThumbnailViewModel ThumbnailViewModel { get; }

	public MainWindowViewModel( IWindowHandleService windowHandleService )
	{
		WindowListViewModel = new WindowListViewModel( windowHandleService );

		ThumbnailViewModel = new ThumbnailViewModel();

		this.WhenActivated( d =>
		{
			this.WhenAnyValue( viewModel => viewModel.OwningWindowHandle )
				.BindTo( ThumbnailViewModel, viewModel => viewModel.DestinationWindow )
				.DisposeWith( d );

			this.WhenAnyValue( viewModel => viewModel.WindowListViewModel.SelectedItem )
				.BindTo( ThumbnailViewModel, viewModel => viewModel.SourceWindow )
				.DisposeWith( d );
		} );
	}
}