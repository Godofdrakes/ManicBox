using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using ManicBox.Services.Interface;
using ReactiveUI;

namespace ManicBox.Services.ViewModel;

public sealed class WindowListViewModel : ReactiveObject, IDisposable
{
	public ReadOnlyObservableCollection<WindowHandleViewModel> Items => _items;

	private readonly ReadOnlyObservableCollection<WindowHandleViewModel> _items;

	private readonly CompositeDisposable _onDispose = new();

	public WindowListViewModel()
	{
		// Does nothing. Used for design-time data and mockups.

		_items = ReadOnlyObservableCollection<WindowHandleViewModel>.Empty;
	}

	public WindowListViewModel( IWindowHandleService windowHandleService )
	{
		ArgumentNullException.ThrowIfNull( windowHandleService );

		windowHandleService
			.EnumerateWindows()
			.Bind( out _items )
			.Subscribe()
			.DisposeWith( _onDispose );
	}

	public void Dispose()
	{
		_onDispose.Dispose();
	}
}