using ManicBox.Services.ViewModel;
using ReactiveUI;

namespace ManicBox.Services.Interface;

public interface IWindowMatchItem : IReactiveObject
{
	bool IsMatch( WindowHandleViewModel viewModel );
}