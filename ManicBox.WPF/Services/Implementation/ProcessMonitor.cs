using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.WPF.Model;
using Microsoft.Extensions.Hosting;
using ReactiveUI;

namespace ManicBox.WPF.Services;

public sealed class ProcessMonitor : IHostedService
{
	private readonly IProcessList _processList;

	private IDisposable? _onStop;

	public ProcessMonitor( IProcessList processList )
	{
		ArgumentNullException.ThrowIfNull( processList );

		_processList = processList;
	}

	public Task StartAsync( CancellationToken cancellationToken )
	{
		using var localProcess = Process.GetCurrentProcess();

		// Don't bother monitoring ourself
		var localProcessId = localProcess.Id;

		_onStop = _processList.Connect( ObservableChangeSet.Create<ProcessInstance, ProcessId>(
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
							.Where( p => p.Id != localProcessId )
							.Where( p => p.MainWindowHandle != IntPtr.Zero )
							.Where( p => !string.IsNullOrEmpty( p.ProcessName ) )
							.Where( p => !string.IsNullOrEmpty( p.MainWindowTitle ) )
							.Select( ProcessInstance.Create )
							.ToList();

						foreach ( ProcessInstance p in allProcesses )
						{
							removedProcesses.Remove( p.Id );
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
			p => p.Id ) );

		return Task.CompletedTask;
	}

	public Task StopAsync( CancellationToken cancellationToken )
	{
		_onStop?.Dispose();
		_onStop = null;

		return Task.CompletedTask;
	}
}