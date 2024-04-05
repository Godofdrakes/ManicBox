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
			// Necessary for the ViewModel to receive WhenActivated
		} );
	}
}