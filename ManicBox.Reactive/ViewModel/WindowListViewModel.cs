using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using ManicBox.Reactive.Services.Interface;
using ReactiveUI;

namespace ManicBox.Reactive.ViewModel;

public sealed class WindowListViewModel : ReactiveObject, IDisposable
{
	public ReadOnlyObservableCollection<WindowHandleViewModel> Items => _items;

	private readonly ReadOnlyObservableCollection<WindowHandleViewModel> _items;

	private readonly CompositeDisposable _onDispose = new();

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