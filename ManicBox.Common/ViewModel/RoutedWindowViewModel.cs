using ManicBox.Common.Services;
using ReactiveUI;

namespace ManicBox.Common.ViewModel;

public class RoutedWindowViewModel : WindowViewModel
{
	public IScreen Screen { get; }

	public RoutedWindowViewModel()
	{
		Screen = new ScreenRoutingService();
	}

	public RoutedWindowViewModel( IScreen screen )
	{
		ArgumentNullException.ThrowIfNull( screen );

		Screen = screen;
	}
}