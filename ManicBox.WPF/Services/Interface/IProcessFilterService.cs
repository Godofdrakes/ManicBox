using DynamicData;
using ManicBox.WPF.Model;

namespace ManicBox.WPF.Services;

public interface IProcessFilterService
{
	ISourceCache<ProcessMatch, Guid> Items { get; }

	IObservable<bool> Matches( ProcessInstance process );
}