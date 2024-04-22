namespace ManicBox.Reactive.Extensions;

public static class TupleExtensions
{
	public static bool HasNull<T1, T2>( this (T1, T2) tuple )
	{
		return tuple.Item1 is not null && tuple.Item2 is not null;
	}
}