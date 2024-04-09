namespace ManicBox.Interop;

[Flags]
internal enum ThumbnailFlags
{
	// Rectangle destination of thumbnail
	RectDestination = 0x0000001,

	// Rectangle source of thumbnail
	RectSource = 0x0000002,

	// Opacity
	Opacity = 0x0000004,

	// Visibility
	Visible = 0x0000008,

	// Displays only the client area of the source
	SourceClientAreaOnly = 0x00000010
}