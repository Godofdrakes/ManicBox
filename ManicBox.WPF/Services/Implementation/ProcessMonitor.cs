using System.Collections.Immutable;
using System.Diagnostics;
using ManicBox.WPF.Model;
using Microsoft.Extensions.Hosting;

namespace ManicBox.WPF.Services;

public sealed class ProcessMonitor : BackgroundService
{
	private readonly IProcessList _processList;

	public ProcessMonitor( IProcessList processList )
	{
		ArgumentNullException.ThrowIfNull( processList );

		_processList = processList;
	}

	protected override Task ExecuteAsync( CancellationToken token )
	{
		return Scan( token );
	}

	private async Task Scan( CancellationToken token )
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				await Task.Delay( TimeSpan.FromSeconds( 1 ), token );

				var processes = Process.GetProcesses();

				try
				{
					_processList.Update( processes
						.Where( process => process.MainWindowHandle != IntPtr.Zero )
						.Where( process => !string.IsNullOrEmpty( process.MainWindowTitle ) )
						.Where( process => !string.IsNullOrEmpty( process.ProcessName ) )
						.Select( ProcessId.Create )
						.ToImmutableList() );
				}
				finally
				{
					foreach ( Process process in processes )
					{
						process.Dispose();
					}
				}
			}
			catch (TaskCanceledException)
			{
				// Clear the process list, the loop will now end
				_processList.Update( ImmutableList<ProcessId>.Empty );
			}
		}
	}
}