using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ManicBox.Interop;

[DebuggerDisplay( "{Value}" )]
[StructLayout( LayoutKind.Sequential )]
[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly struct HANDLE : IEquatable<HANDLE>
{
	private readonly IntPtr Value;

	public bool IsValid => Value != default;

	public bool IsNull => Value == default;

	public static implicit operator IntPtr( HANDLE handle ) => handle.Value;

	public static HANDLE Null => default;

	public HANDLE( IntPtr value ) => Value = value;

	public static bool operator ==( HANDLE left, HANDLE right )
	{
		return left.Value == right.Value;
	}

	public static bool operator !=( HANDLE left, HANDLE right )
	{
		return !(left == right);
	}

	public bool Equals( HANDLE other )
	{
		return Value == other.Value;
	}

	public override bool Equals( object? obj )
	{
		return obj is HANDLE other && Equals( other );
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString() => $"0x{Value:x}";
}