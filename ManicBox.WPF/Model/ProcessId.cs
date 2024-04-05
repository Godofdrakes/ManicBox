using System.Diagnostics;

namespace ManicBox.WPF.Model;

public record ProcessId( string ProcessName, string WindowTitle )
{
	public static ProcessId Create( Process process )
	{
		ArgumentNullException.ThrowIfNull( process );

		return new ProcessId( process.ProcessName, process.MainWindowTitle );
	}
}