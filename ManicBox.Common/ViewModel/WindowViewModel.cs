using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Common.ViewModel;

public class WindowViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive] public string? Title { get; set; }
}