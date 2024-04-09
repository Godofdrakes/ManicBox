using System.Drawing;
using System.Windows;

namespace ManicBox.Preview;

public static class UIElementEx
{
	public static Rectangle GetRect( this UIElement element )
	{
		return new Rectangle( 0, 0, (int)element.RenderSize.Width, (int)element.RenderSize.Height );
	}
}