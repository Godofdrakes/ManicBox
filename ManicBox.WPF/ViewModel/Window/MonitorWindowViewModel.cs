using ManicBox.Common.ViewModel;
using ManicBox.Services.Interface;
using ManicBox.Services.ViewModel;

namespace ManicBox.WPF.ViewModel;

public class MonitorWindowViewModel : WindowViewModel
{
	public WindowMatchServiceViewModel WindowMatchService { get; }

	public WindowMonitorServiceViewModel WindowMonitorService { get; }

	public MonitorWindowViewModel(
		IWindowMonitorService windowMonitor,
		IWindowMatchService windowMatch )
	{
		WindowMatchService = new WindowMatchServiceViewModel( windowMatch );
		WindowMonitorService = new WindowMonitorServiceViewModel( windowMonitor )
		{
			Filter = windowMatch.HasMatch
		};
	}
}