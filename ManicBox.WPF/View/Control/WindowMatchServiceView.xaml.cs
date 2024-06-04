using ReactiveUI;

namespace ManicBox.WPF.View.Control;

public partial class WindowMatchServiceView
{
	public WindowMatchServiceView()
	{
		InitializeComponent();

		this.WhenActivated( onDispose =>
		{
			
		} );
	}
}