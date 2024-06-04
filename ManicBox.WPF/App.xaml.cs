using System.Windows;
using ManicBox.Common.Extensions;
using ManicBox.WPF.View;
using ManicBox.WPF.ViewModel;
using Microsoft.Extensions.DependencyInjection;

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
		this.MainWindow = _serviceProvider.CreateWindow<MainWindow>( window =>
		{
			window.ViewModel = ActivatorUtilities.CreateInstance<MainWindowViewModel>( _serviceProvider );
		} );

		this.MainWindow.Show();
	}
}