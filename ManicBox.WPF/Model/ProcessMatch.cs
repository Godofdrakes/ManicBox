using System.Diagnostics.Contracts;

namespace ManicBox.WPF.Model;

public struct ProcessMatch
{
	public Guid Guid { get; }
	public string ProcessName { get; }
	public string WindowTitle { get; }

	public ProcessMatch( Guid guid, string processName, string? windowTitle = default )
	{
		ArgumentException.ThrowIfNullOrEmpty( processName );

		Guid = guid;
		ProcessName = processName;
		WindowTitle = windowTitle ?? string.Empty;
	}

	[Pure]
	public bool Matches( ProcessInstance process )
	{
		ArgumentNullException.ThrowIfNull( process );

		const StringComparison mode = StringComparison.CurrentCultureIgnoreCase;

		var nameMatch = process.ProcessName.StartsWith( ProcessName, mode );
		var titleMatch = process.WindowTitle.StartsWith( WindowTitle, mode );

		return nameMatch && titleMatch;
	}
}