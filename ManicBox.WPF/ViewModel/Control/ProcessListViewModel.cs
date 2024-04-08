using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.WPF.Model;
using ManicBox.WPF.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.WPF.ViewModel;

public class ProcessListViewModel : UserControlViewModel
{
	[Reactive] public Func<ProcessInstance, IObservable<bool>>? Filter { get; set; }

	public ReadOnlyObservableCollection<ProcessInstance> Items => _items;

	private readonly ReadOnlyObservableCollection<ProcessInstance> _items;

	private readonly SourceCache<ProcessInstance, ProcessId> _source = new( p => p.GetProcessId() );

	public ProcessListViewModel( IProcessListService processListService )
	{
		Filter = static _ => Observable.Never<bool>().StartWith( true );

		_source.Connect()
			.Bind( out _items )
			.Subscribe();

		this.WhenActivated( d =>
		{
			processListService.Processes.Connect()
				.FilterOnObservable( p => this
					.WhenAnyValue( view => view.Filter )
					.Select( f => f ?? NullFilter )
					.Select( f => f.Invoke( p ) )
					.Switch() )
				.SubscribeOn( RxApp.MainThreadScheduler )
				.ObserveOn( RxApp.MainThreadScheduler )
				.PopulateInto( _source )
				.DisposeWith( d );
		} );
	}

	private static IObservable<bool> NullFilter( ProcessInstance processInstance )
	{
		return Observable.Never<bool>().StartWith( true );
	}
}