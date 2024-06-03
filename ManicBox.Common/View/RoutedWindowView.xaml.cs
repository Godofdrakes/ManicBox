using ReactiveUI;

namespace ManicBox.Common.View;

public partial class RoutedWindowView
{
	public RoutedWindowView()
	{
		InitializeComponent();

		this.WhenActivated( onDispose =>
		{
			this.OneWayBind( ViewModel,
				vm => vm.Screen.Router,
				v => v.RoutedViewHost.Router );
		} );
	}
}