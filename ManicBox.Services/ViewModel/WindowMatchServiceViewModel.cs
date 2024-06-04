using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using ManicBox.Services.Extensions;
using ManicBox.Services.Interface;
using ReactiveUI;

namespace ManicBox.Services.ViewModel;

public sealed class WindowMatchServiceViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	public ReadOnlyObservableCollection<IWindowMatchItem> Items => _items;

	private readonly ReadOnlyObservableCollection<IWindowMatchItem> _items;

	public WindowMatchServiceViewModel( IWindowMatchService windowMatchService )
	{
		ArgumentNullException.ThrowIfNull( windowMatchService );

		windowMatchService.GetItems()
			.Publish( out var connection )
			.Bind( out _items )
			.Subscribe();

		this.WhenActivated( onDispose =>
		{
			connection.Connect().DisposeWith( onDispose );
		} );
	}
}