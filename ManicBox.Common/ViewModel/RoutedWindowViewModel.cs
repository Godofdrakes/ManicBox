using System.Reactive;
using ManicBox.Common.Services;
using ReactiveUI;

namespace ManicBox.Common.ViewModel;

public class RoutedWindowViewModel : WindowViewModel
{
	public IScreen Screen { get; }

	public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Reset { get; }
	public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Push { get; }
	public ReactiveCommand<Unit, IRoutableViewModel> Pop { get; }

	public RoutedWindowViewModel() : this( new ScreenRoutingService() ) { }

	public RoutedWindowViewModel( IScreen screen )
	{
		ArgumentNullException.ThrowIfNull( screen );

		Screen = screen;

		Reset = ReactiveCommand.CreateFromObservable( ( IRoutableViewModel viewModel ) =>
			Screen.Router.NavigateAndReset.Execute( viewModel ) );
		Push = ReactiveCommand.CreateFromObservable( ( IRoutableViewModel viewModel ) =>
			Screen.Router.Navigate.Execute( viewModel ) );
		Pop = ReactiveCommand.CreateFromObservable( () =>
			Screen.Router.NavigateBack.Execute() );
	}
}