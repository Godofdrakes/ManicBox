using ManicBox.Reactive.ViewModelBase;
using ManicBox.WPF.Services;

namespace ManicBox.WPF.ViewModel;

public class MainWindowViewModel : WindowViewModel
{
	public ProcessListViewModel ProcessListViewModel { get; }

	public ProcessFilterViewModel ProcessFilterViewModel { get; }

	public MainWindowViewModel(
		IProcessListService processListService,
		IProcessFilterService processFilterService )
	{
		ProcessListViewModel = new ProcessListViewModel( processListService )
		{
			Filter = processFilterService.Matches
		};

		ProcessFilterViewModel = new ProcessFilterViewModel( processFilterService );
	}
}