namespace ManicBox.Interop.Common;

public struct Margins
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;

	public Margins( int left, int top, int right, int bottom )
	{
		Left = left;
		Top = top;
		Right = right;
		Bottom = bottom;
	}

	public static readonly Margins Fill = new( -1, -1, -1, -1 );
}
