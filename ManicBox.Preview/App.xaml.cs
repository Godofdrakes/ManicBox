using System.Reflection;
using System.Windows;
using ManicBox.Common.Extensions;
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
	private readonly IServiceProvider _serviceProvider;

	public App( IServiceProvider serviceProvider )
	{
		ArgumentNullException.ThrowIfNull( serviceProvider );

		_serviceProvider = serviceProvider;

		// Needed because App isn't the main entry point anymore
		InitializeComponent();
	}

	private void App_OnStartup( object sender, StartupEventArgs e )
	{
		this.MainWindow = _serviceProvider.CreateWindow<MainWindow>( window =>
		{
			window.ViewModel = new MainWindowViewModel( _windowHandleService )
			{
				Title = Assembly.GetExecutingAssembly().FullName
			};
		} );

		this.MainWindow.Show();
	}
}