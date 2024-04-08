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
	public ProcessFilterViewModel ProcessFilterViewModel { get; }

	public ReadOnlyObservableCollection<ProcessInstance> ProcessList => _processList;

	private readonly ReadOnlyObservableCollection<ProcessInstance> _processList;

	private readonly SourceCache<ProcessInstance, ProcessId> _cache = new( p => p.GetProcessId() );

	public MainWindowViewModel(
		ILogger<MainWindowViewModel> logger,
		IProcessListService processListService,
		IProcessFilterService processFilterService )
	{
		ProcessFilterViewModel = new ProcessFilterViewModel( processFilterService );

		_cache.Connect()
			.Bind( out _processList )
			.Subscribe();

		this.WhenActivated( d =>
		{
			processListService.Processes
				.Connect()
				.SubscribeOn( RxApp.MainThreadScheduler )
				.ObserveOn( RxApp.MainThreadScheduler )
				.PopulateInto( _cache )
				.DisposeWith( d );
		} );
	}
}