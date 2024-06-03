using System.Reflection;
using System.Windows;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace ManicBox.Common.Extensions;

public static class HostBuilderExtensions
{
	public static IHostBuilder ConfigureWpf<T>( this IHostBuilder hostBuilder )
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

		hostBuilder.UseWpfLifetime();

		return hostBuilder;
	}

	public static IHost InitializeSplat( this IHost host )
	{
		// Splat needs this for reasons
		host.Services.UseMicrosoftDependencyResolver();

		return host;
	}
}