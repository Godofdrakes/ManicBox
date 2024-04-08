using DynamicData;
using ManicBox.WPF.Model;

namespace ManicBox.WPF.Services;

public interface IProcessList
{
	IObservableCache<ProcessInstance, ProcessId> Processes { get; }

	IDisposable Connect( IObservable<IChangeSet<ProcessInstance, ProcessId>> observable );
}