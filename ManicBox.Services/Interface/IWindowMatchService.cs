using DynamicData;
using ManicBox.Services.ViewModel;

namespace ManicBox.Services.Interface;

public interface IWindowMatchService
{
	IObservable<IChangeSet<IWindowMatchItem, IWindowMatchItem>> GetItems();

	void Add( IWindowMatchItem item );

	void Remove( IWindowMatchItem item );

	IObservable<bool> HasMatch( WindowHandleViewModel viewModel );
}