using DynamicData;
using ManicBox.Interop;
using ManicBox.Services.ViewModel;

namespace ManicBox.Services.Interface;

public interface IWindowMonitorService
{
	IObservable<IChangeSet<WindowHandleViewModel, HWND>> GetWindows();
}