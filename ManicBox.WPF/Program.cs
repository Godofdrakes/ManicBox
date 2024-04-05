using System.Reflection;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using ManicBox.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace ManicBox.WPF;

public static class Program
{
	public static void Main( string[] args )
	{
		var host = Host.CreateDefaultBuilder( args )
			.ConfigureWpf<App>()
			.Build();

		host.InitializeSplat()
			.Run();
	}

	private static IHostBuilder ConfigureWpf<T>( this IHostBuilder hostBuilder )
		where T : Application
	{
		hostBuilder.ConfigureWpf( builder => { builder.UseApplication<T>(); } );

		hostBuilder.ConfigureServices( services =>
		{
			// Splat needs all this for reasons
			services.UseMicrosoftDependencyResolver();

			var locator = Locator.CurrentMutable;
			locator.InitializeSplat();
			locator.InitializeReactiveUI();
		} );

		hostBuilder.ConfigureServices( services => services
			.AddHostedServices( Assembly.GetExecutingAssembly() )
			.AddSingleton<IProcessList, ProcessList>() );

		hostBuilder.UseWpfLifetime();

		return hostBuilder;
	}

	private static IServiceCollection AddHostedServices( this IServiceCollection services, Assembly assembly )
	{
		ArgumentNullException.ThrowIfNull( services );
		ArgumentNullException.ThrowIfNull( assembly );

		var hostedServices = assembly.ExportedTypes
			.Where( type => type.IsAssignableTo( typeof(IHostedService) ) )
			.Where( type => !type.IsAbstract )
			.Select( type => ServiceDescriptor.Singleton( typeof(IHostedService), type ) );

		foreach ( ServiceDescriptor service in hostedServices )
		{
			services.TryAddEnumerable( service );
		}

		return services;
	}

	private static IHost InitializeSplat( this IHost host )
	{
		// Splat needs this for reasons
		host.Services.UseMicrosoftDependencyResolver();

		return host;
	}
}