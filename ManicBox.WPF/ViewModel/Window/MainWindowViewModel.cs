using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.WPF.Model;
using ManicBox.WPF.Services;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.WPF.ViewModel;

public class MainWindowViewModel : WindowViewModel
{
	[Reactive] public ReadOnlyObservableCollection<ProcessId> ProcessList { get; private set; }

	public MainWindowViewModel( ILogger<MainWindowViewModel> logger, IProcessList processList )
	{
		ProcessList = ReadOnlyObservableCollection<ProcessId>.Empty;

		this.WhenActivated( d =>
		{
			processList.AllProcesses()
				.OnItemAdded( id => logger.LogWarning( "Added {Id}", id ) )
				.OnItemRemoved( id => logger.LogWarning( "Removed {Id}", id ) )
				.SubscribeOn( RxApp.MainThreadScheduler )
				.ObserveOn( RxApp.MainThreadScheduler )
				.Bind( out var list )
				.Subscribe()
				.DisposeWith( d );

			Disposable.Create( () => ProcessList = ReadOnlyObservableCollection<ProcessId>.Empty )
				.DisposeWith( d );

			ProcessList = list;
		} );
	}
}