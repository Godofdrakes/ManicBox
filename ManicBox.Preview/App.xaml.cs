using System.Reflection;
using System.Windows;
using ManicBox.Common.Extensions;
using ManicBox.Preview.View;
using ManicBox.Preview.ViewModel;
using ManicBox.Services.Implementation;
using ManicBox.Services.Interface;

namespace ManicBox.Preview;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private readonly IWindowMonitorService _windowMonitorService = new WindowMonitorService();
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
			window.ViewModel = new MainWindowViewModel( _windowMonitorService )
			{
				Title = Assembly.GetExecutingAssembly().FullName
			};
		} );

		this.MainWindow.Show();
	}
}