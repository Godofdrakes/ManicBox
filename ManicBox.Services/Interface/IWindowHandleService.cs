using DynamicData;
using ManicBox.Interop;
using ManicBox.Interop.Common;
using ManicBox.Reactive.ViewModel;

namespace ManicBox.Reactive.Services.Interface;

public interface IWindowHandleService
{
	IObservable<IChangeSet<WindowHandleViewModel, HWND>> EnumerateWindows();

	IObservable<Margins> OnMoveSize( HWND hWnd );

	IObservable<string> OnTitleChange( HWND hWnd );

	IObservable<bool> IsForeground( HWND hWnd );
}