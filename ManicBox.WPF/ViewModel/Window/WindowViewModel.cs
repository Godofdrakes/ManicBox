using ReactiveUI;

namespace ManicBox.WPF.ViewModel;

public class WindowViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();
}