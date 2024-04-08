using System.Diagnostics;

namespace ManicBox.WPF.Model;

public class ProcessInstance
{
	public ProcessId Id { get; }

	public string WindowTitle { get; }

	public ProcessInstance( ProcessId processId, string windowTitle )
	{
		ArgumentNullException.ThrowIfNull( processId );
		ArgumentException.ThrowIfNullOrEmpty( windowTitle );

		Id = processId;
		WindowTitle = windowTitle;
	}

	public static ProcessInstance Create( Process process )
	{
		return new ProcessInstance( ProcessId.Create( process ), process.MainWindowTitle );
	}
}