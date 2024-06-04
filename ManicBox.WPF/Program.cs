using System.Reflection;
using ManicBox.Common.Extensions;
using ManicBox.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ManicBox.WPF;

public static class Program
{
	public static void Main( string[] args )
	{
		var host = Host.CreateDefaultBuilder( args )
			.ConfigureWpf<App>()
			.ConfigureServices()
			.Build();

		host.InitializeSplat()
			.Run();
	}

	private static IHostBuilder ConfigureServices( this IHostBuilder hostBuilder )
	{
		hostBuilder.ConfigureServices( services => services
			.AddHostedServices( Assembly.GetExecutingAssembly() )
			.AddSingleton<IProcessListService, ProcessListService>()
			.AddSingleton<IProcessFilterService, ProcessFilterService>() );

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
}