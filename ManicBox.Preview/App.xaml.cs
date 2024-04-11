using System.Reflection;
using System.Windows;
using ManicBox.Reactive.ViewModel;

namespace ManicBox.Preview;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private void App_OnStartup( object sender, StartupEventArgs e )
	{
		this.MainWindow = new MainWindow()
		{
			ViewModel = new WindowViewModel()
			{
				Title = Assembly.GetExecutingAssembly().FullName
			}
		};

		this.MainWindow.Show();
	}
}