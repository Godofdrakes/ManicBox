using System.Collections.Immutable;
using DynamicData;
using ManicBox.WPF.Model;

namespace ManicBox.WPF.Services;

public class ProcessList : IProcessList
{
	private readonly SourceCache<ProcessId, ProcessId> _source;

	public ProcessList()
	{
		_source = new SourceCache<ProcessId, ProcessId>( id => id );
	}

	public IObservable<IChangeSet<ProcessId, ProcessId>> AllProcesses()
	{
		return _source.Connect();
	}

	public void Update( IReadOnlyCollection<ProcessId> allProcesses )
	{
		var all = allProcesses.ToImmutableHashSet();

		foreach ( ProcessId id in _source.Items )
		{
			if ( !all.Contains( id ) )
			{
				_source.RemoveKey( id );
			}
		}

		var known = _source.Items.ToImmutableHashSet();

		foreach ( ProcessId id in allProcesses )
		{
			if ( !known.Contains( id ) )
			{
				_source.AddOrUpdate( id );
			}
		}
	}
}