using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.WPF.Model;
using ManicBox.WPF.Services;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace ManicBox.WPF.ViewModel;

public class MainWindowViewModel : WindowViewModel
{
	public ReadOnlyObservableCollection<ProcessInstance> ProcessList => _processList;

	private readonly ReadOnlyObservableCollection<ProcessInstance> _processList;

	private readonly SourceCache<ProcessInstance, ProcessId> _cache = new( p => p.Id );

	public MainWindowViewModel( ILogger<MainWindowViewModel> logger, IProcessList processList )
	{
		_cache.Connect()
			.Bind( out _processList )
			.Subscribe();

		this.WhenActivated( d =>
		{
			processList.Processes
				.Connect()
				.OnItemAdded( id => logger.LogWarning( "Added {Process}", id.Id.Name ) )
				.OnItemRemoved( id => logger.LogWarning( "Removed {Process}", id.Id.Name ) )
				.SubscribeOn( RxApp.MainThreadScheduler )
				.ObserveOn( RxApp.MainThreadScheduler )
				.PopulateInto( _cache )
				.DisposeWith( d );
		} );
	}
}