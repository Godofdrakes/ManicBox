using System.Diagnostics;

namespace ManicBox.WPF.Model;

public class ProcessInstance
{
	public string ProcessName { get; }

	public int ProcessId { get; }

	public string WindowTitle { get; }

	public ProcessInstance( string name, int id, string title )
	{
		ArgumentException.ThrowIfNullOrEmpty( name );
		ArgumentException.ThrowIfNullOrEmpty( title );

		ProcessName = name;
		ProcessId = id;
		WindowTitle = title;
	}

	public ProcessId GetProcessId()
	{
		return new ProcessId( ProcessName, ProcessId );
	}

	public static ProcessInstance Create( Process process )
	{
		return new ProcessInstance( process.ProcessName, process.Id, process.MainWindowTitle );
	}
}