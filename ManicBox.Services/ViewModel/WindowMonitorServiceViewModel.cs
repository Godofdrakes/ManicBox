using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.Services.Extensions;
using ManicBox.Services.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Services.ViewModel;

public sealed class WindowMonitorServiceViewModel : ReactiveObject, IActivatableViewModel
{
	public delegate IObservable<bool> MatchPredicate( WindowHandleViewModel viewModel );

	public ViewModelActivator Activator { get; } = new();

	[Reactive] public MatchPredicate? Filter { get; set; }

	public ReadOnlyObservableCollection<WindowHandleViewModel> Items => _items;

	private readonly ReadOnlyObservableCollection<WindowHandleViewModel> _items;

	public WindowMonitorServiceViewModel()
	{
		// Does nothing. Used for design-time data and mockups.

		_items = ReadOnlyObservableCollection<WindowHandleViewModel>.Empty;
	}

	public WindowMonitorServiceViewModel( IWindowMonitorService windowMonitorService )
	{
		ArgumentNullException.ThrowIfNull( windowMonitorService );

		windowMonitorService
			.GetWindows()
			.Publish( out var connection )
			.FilterOnObservable( viewModel => this
				.WhenAnyValue( vm => vm.Filter )
				.Select( filter => filter?.Invoke( viewModel ) ?? NullFilter() )
				.Switch() )
			.Bind( out _items )
			.Subscribe();

		this.WhenActivated( onDispose => connection.Connect().DisposeWith( onDispose ) );
	}

	private static IObservable<bool> NullFilter()
	{
		return Observable.Never<bool>().StartWith( true );
	}
}