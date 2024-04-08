using System.Diagnostics;
using DynamicData;
using ManicBox.WPF.Model;
using Microsoft.Extensions.Hosting;

namespace ManicBox.WPF.Services;

public sealed class ProcessMonitorService : IHostedService
{
	private readonly IProcessListService _processListService;

	private IDisposable? _onStop;

	public ProcessMonitorService( IProcessListService processListService )
	{
		ArgumentNullException.ThrowIfNull( processListService );

		_processListService = processListService;
	}

	public Task StartAsync( CancellationToken cancellationToken )
	{
		_onStop = _processListService.Connect( ObservableChangeSet.Create<ProcessInstance, ProcessId>(
			async ( cache, token ) =>
			{
				while ( !token.IsCancellationRequested )
				{
					await Task.Delay( TimeSpan.FromSeconds( 1 ), cancellationToken );

					var processes = Process.GetProcesses();

					try
					{
						var removedProcesses = cache.Keys.ToHashSet();

						var allProcesses = processes
							.Where( p => p.MainWindowHandle != IntPtr.Zero )
							.Where( p => !string.IsNullOrEmpty( p.ProcessName ) )
							.Where( p => !string.IsNullOrEmpty( p.MainWindowTitle ) )
							.Select( ProcessInstance.Create )
							.ToList();

						foreach ( ProcessInstance p in allProcesses )
						{
							removedProcesses.Remove( p.GetProcessId() );
						}

						cache.Edit( items =>
						{
							items.RemoveKeys( removedProcesses );

							items.AddOrUpdate( allProcesses );
						} );
					}
					finally
					{
						foreach ( Process process in processes )
						{
							process.Dispose();
						}
					}
				}
			},
			p => p.GetProcessId() ) );

		return Task.CompletedTask;
	}

	public Task StopAsync( CancellationToken cancellationToken )
	{
		_onStop?.Dispose();
		_onStop = null;

		return Task.CompletedTask;
	}
}