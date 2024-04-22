using System.Reflection;
using System.Windows;
using ManicBox.Preview.View;
using ManicBox.Preview.ViewModel;
using ManicBox.Reactive.Services.Implementation;
using ManicBox.Reactive.Services.Interface;

namespace ManicBox.Preview;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private readonly IWindowHandleService _windowHandleService = new WindowHandleService();

	private void App_OnStartup( object sender, StartupEventArgs e )
	{
		this.MainWindow = new MainWindow()
		{
			ViewModel = new MainWindowViewModel( _windowHandleService )
			{
				Title = Assembly.GetExecutingAssembly().FullName
			}
		};

		this.MainWindow.Show();
	}
}