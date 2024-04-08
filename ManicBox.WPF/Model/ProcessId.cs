using System.Diagnostics;

namespace ManicBox.WPF.Model;

public record ProcessId( string Name, int Id )
{
	public static ProcessId Create( Process process )
	{
		ArgumentNullException.ThrowIfNull( process );

		return new ProcessId( process.ProcessName, process.Id );
	}
}