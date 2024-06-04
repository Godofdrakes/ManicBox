using ManicBox.Services.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Services.ViewModel;

public class WindowTitleMatchViewModel : ReactiveObject, IWindowMatchItem
{
	[Reactive] public string ProcessName { get; set; } = string.Empty;
	[Reactive] public string WindowTitle { get; set; } = string.Empty;

	public bool IsMatch( WindowHandleViewModel viewModel )
	{
		ArgumentNullException.ThrowIfNull( viewModel );

		if ( !string.IsNullOrEmpty( ProcessName ) )
		{
			if ( !viewModel.ProcessName.StartsWith( ProcessName ) )
			{
				return false;
			}
		}

		if ( !string.IsNullOrEmpty( WindowTitle ) )
		{
			if ( !viewModel.WindowTitle.StartsWith( WindowTitle ) )
			{
				return false;
			}
		}

		return true;
	}
}