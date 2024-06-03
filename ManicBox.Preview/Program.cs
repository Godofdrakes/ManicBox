using ManicBox.Common.Extensions;
using Microsoft.Extensions.Hosting;

namespace ManicBox.Preview;

public static class Program
{
	public static void Main( string[] args )
	{
		IHost host = Host.CreateDefaultBuilder( args )
			.ConfigureWpf<App>()
			.Build();

		host.InitializeSplat()
			.Run();
	}
}