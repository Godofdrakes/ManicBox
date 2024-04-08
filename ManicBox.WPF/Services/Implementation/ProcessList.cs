using DynamicData;
using ManicBox.WPF.Model;

namespace ManicBox.WPF.Services;

public class ProcessList : IProcessList
{
	public IObservableCache<ProcessInstance, ProcessId> Processes => _source.AsObservableCache();

	private readonly SourceCache<ProcessInstance, ProcessId> _source;

	public ProcessList()
	{
		_source = new SourceCache<ProcessInstance, ProcessId>( process => process.GetProcessId() );
	}

	public IDisposable Connect( IObservable<IChangeSet<ProcessInstance, ProcessId>> observable )
	{
		return observable.PopulateInto( _source );
	}
}