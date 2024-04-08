using System.Reactive;
using DynamicData;
using ManicBox.WPF.Model;
using ManicBox.WPF.Services;
using ReactiveUI;

namespace ManicBox.WPF.ViewModel;

public class ProcessMatchViewModel : UserControlViewModel
{
	public Guid Guid => _match.Guid;
	public string ProcessName => _match.ProcessName;
	public string WindowTitle => _match.WindowTitle;

	public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
	public ReactiveCommand<Unit, Unit> EditCommand { get; }

	private readonly ProcessMatch _match;

	public ProcessMatchViewModel( IProcessFilterService processFilterService, ProcessMatch match )
	{
		_match = match;

		RemoveCommand = ReactiveCommand.Create( () => processFilterService.Items.RemoveKey( _match.Guid ) );
		EditCommand = ReactiveCommand.Create( () => { } );
	}
}