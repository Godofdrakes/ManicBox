using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.WPF.Model;
using ManicBox.WPF.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.WPF.ViewModel;

public class ProcessFilterViewModel : UserControlViewModel
{
	[Reactive] public string ProcessName { get; set; } = string.Empty;
	[Reactive] public string WindowTitle { get; set; } = string.Empty;

	public ReadOnlyObservableCollection<ProcessMatchViewModel> Items => _items;

	public ReactiveCommand<Unit, Unit> AddCommand { get; }

	private readonly ReadOnlyObservableCollection<ProcessMatchViewModel> _items;

	private readonly SourceCache<ProcessMatchViewModel, Guid> _source = new( match => match.Guid );

	public ProcessFilterViewModel( IProcessFilterService processFilterService )
	{
		ArgumentNullException.ThrowIfNull( processFilterService );

		AddCommand = ReactiveCommand.Create( () =>
		{
			var match = new ProcessMatch( Guid.NewGuid(), ProcessName, WindowTitle );

			ProcessName = string.Empty;
			WindowTitle = string.Empty;

			processFilterService.Items.AddOrUpdate( match );
		} );

		_source.Connect()
			.Bind( out _items )
			.Subscribe();

		this.WhenActivated( d =>
		{
			processFilterService.Items.Connect()
				.Transform( match => new ProcessMatchViewModel( processFilterService, match ) )
				.SubscribeOn( RxApp.MainThreadScheduler )
				.ObserveOn( RxApp.MainThreadScheduler )
				.PopulateInto( _source )
				.DisposeWith( d );
		} );
	}
}