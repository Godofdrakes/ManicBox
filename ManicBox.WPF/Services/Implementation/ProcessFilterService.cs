using DynamicData;
using ManicBox.WPF.Model;

namespace ManicBox.WPF.Services;

public class ProcessFilterService : IProcessFilterService
{
	public ISourceCache<ProcessMatch, Guid> Items => _source;

	private readonly SourceCache<ProcessMatch, Guid> _source = new( match => match.Guid );

	public ProcessFilterService()
	{
		// @todo: save/load
		_source.AddOrUpdate( new ProcessMatch( Guid.NewGuid(), "ManicBox", "ManicBox.Mock" ) );
		_source.AddOrUpdate( new ProcessMatch( Guid.NewGuid(), "RuneLite" ) );
		_source.AddOrUpdate( new ProcessMatch( Guid.NewGuid(), "UnrealEditor" ) );
	}

	public IObservable<bool> Matches( ProcessInstance process )
	{
		return _source.Connect()
			.QueryWhenChanged( cache => cache.Items
				.Any( match => match.Matches( process ) ) );
	}
}