using System.Configuration;
using System.Data;
using System.Windows;

namespace ManicBox.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private readonly IServiceProvider _serviceProvider;

	public App( IServiceProvider serviceProvider )
	{
		// Needed because App isn't the main entry point anymore
		InitializeComponent();

		_serviceProvider = serviceProvider;
	}

	private void App_OnStartup( object sender, StartupEventArgs e )
	{
		this.MainWindow = new MainWindow();
		this.MainWindow.Show();
	}
}