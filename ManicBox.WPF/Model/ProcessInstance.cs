using System.Diagnostics;
using ManicBox.WPF.Exceptions;

namespace ManicBox.WPF.Model;

public class ProcessInstance
{
	public string ProcessName { get; }

	public int ProcessId { get; }

	public string WindowTitle { get; }

	public IntPtr WindowHandle { get; }

	public ProcessInstance( string name, int id, string title, IntPtr window )
	{
		ArgumentException.ThrowIfNullOrEmpty( name );
		// Check id? Pretty sure zero is a valid process id.
		ArgumentException.ThrowIfNullOrEmpty( title );
		IntPtrException.ThrowIfZero( window );

		ProcessName = name;
		ProcessId = id;
		WindowTitle = title;
		WindowHandle = window;
	}

	public ProcessId GetProcessId()
	{
		return new ProcessId( ProcessName, ProcessId );
	}

	public static ProcessInstance Create( Process process )
	{
		return new ProcessInstance(
			process.ProcessName,
			process.Id,
			process.MainWindowTitle,
			process.MainWindowHandle );
	}
}