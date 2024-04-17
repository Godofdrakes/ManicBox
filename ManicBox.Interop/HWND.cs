using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

[DebuggerDisplay( "{Value}" )]
[StructLayout( LayoutKind.Sequential )]
public struct HWND : IEquatable<HWND>
{
	private readonly IntPtr Value;

	public bool IsValid => Value != default;

	public bool IsNull => Value == default;

	public static implicit operator IntPtr( HWND hwnd ) => hwnd.Value;

	public static HWND Null => default;

	public HWND( IntPtr value ) => Value = value;

	public bool Equals( HWND other )
	{
		return Value == other.Value;
	}

	public override bool Equals( object? obj )
	{
		return obj is HWND other && Equals( other );
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString() => $"0x{Value:x}";
}