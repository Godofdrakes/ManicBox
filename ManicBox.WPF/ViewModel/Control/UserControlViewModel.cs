using ReactiveUI;

namespace ManicBox.WPF.ViewModel;

public class UserControlViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();
}