using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ManicBox.WPF.Exceptions;

public class IntPtrException
{
	public static void ThrowIfZero( IntPtr param, [CallerMemberName] string paramName = "" )
	{
		if ( param == IntPtr.Zero )
		{
			ThrowArgumentException( paramName );
		}
	}

	[DoesNotReturn]
	private static void ThrowArgumentException( string paramName )
	{
		throw new ArgumentException( default, paramName );
	}
}