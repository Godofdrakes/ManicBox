using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using ManicBox.Preview.Extensions;
using ManicBox.Reactive.Services.Interface;
using ManicBox.Reactive.ViewModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Preview.ViewModel;

public class WindowListViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive] public WindowHandleViewModel? SelectedItem { get; set; }

	public ReadOnlyObservableCollection<WindowHandleViewModel> Windows => _windows;

	private readonly ReadOnlyObservableCollection<WindowHandleViewModel> _windows;

	public WindowListViewModel( IWindowHandleService windowHandleService )
	{
		windowHandleService
			.EnumerateWindows()
			.Publish( out var connection )
			.Bind( out _windows )
			.Subscribe();

		this.WhenActivated( d => connection.Connect().DisposeWith( d ) );
	}
}