using ReactiveUI;

namespace ManicBox.Reactive.ViewModelBase;

public class WindowViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	public string? Title { get; set; }
}