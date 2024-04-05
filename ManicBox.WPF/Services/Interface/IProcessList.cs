using DynamicData;
using ManicBox.WPF.Model;

namespace ManicBox.WPF.Services;

public interface IProcessList
{
	IObservable<IChangeSet<ProcessId, ProcessId>> AllProcesses();

	void Update( IReadOnlyCollection<ProcessId> allProcesses );
}