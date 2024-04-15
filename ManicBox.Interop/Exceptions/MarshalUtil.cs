using System.Runtime.InteropServices;

namespace ManicBox.Interop.Exceptions;

public static class MarshalUtil
{
	public static void ThrowLastError()
	{
		Marshal.ThrowExceptionForHR( Marshal.GetLastPInvokeError() );
	}
}