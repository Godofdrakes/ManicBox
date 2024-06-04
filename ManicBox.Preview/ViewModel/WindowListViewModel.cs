using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;
using ManicBox.Services.Extensions;
using ManicBox.Services.Interface;
using ManicBox.Services.ViewModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Preview.ViewModel;

public class WindowListViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive] public WindowHandleViewModel? SelectedItem { get; set; }

	public ReadOnlyObservableCollection<WindowHandleViewModel> Windows => _windows;

	private readonly ReadOnlyObservableCollection<WindowHandleViewModel> _windows;

	public WindowListViewModel( IWindowMonitorService windowMonitorService )
	{
		windowMonitorService
			.GetWindows()
			.Publish( out var connection )
			.Bind( out _windows )
			.Subscribe();

		this.WhenActivated( d => connection.Connect().DisposeWith( d ) );
	}
}