using System.Windows.Controls;

namespace ManicBox.Mock;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void TextBoxBase_OnTextChanged( object sender, TextChangedEventArgs e )
	{
		if ( sender is TextBox textBox )
		{
			if ( !string.IsNullOrEmpty( textBox.Text ) )
			{
				this.Title = textBox.Text;
				return;
			}
		}

		this.Title = "ManicBox.Mock";
	}
}