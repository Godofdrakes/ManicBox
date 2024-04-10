using System.Diagnostics.CodeAnalysis;

namespace ManicBox.Interop.Exceptions;

public static class HResult
{
	public static void ThrowIfError( int errorCode )
	{
		if ( errorCode != 0 )
		{
			Throw( errorCode );
		}
	}

	[DoesNotReturn]
	private static void Throw( int errorCode )
	{
		throw new System.Runtime.InteropServices.ExternalException( "-", errorCode );
	}
}