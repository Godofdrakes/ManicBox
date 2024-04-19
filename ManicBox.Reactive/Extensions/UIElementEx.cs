using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ManicBox.Reactive.Extensions;

public static class UIElementEx
{
	private sealed class UIElementObservables
	{
		public readonly IObservable<EventPattern<object>> LayoutUpdated;

		public UIElementObservables( UIElement uiElement )
		{
			ArgumentNullException.ThrowIfNull( uiElement );

			LayoutUpdated = Observable.FromEventPattern(
					handler => uiElement.LayoutUpdated += handler,
					handler => uiElement.LayoutUpdated -= handler )
				.Publish()
				.RefCount();
		}
	}

	private static readonly ConditionalWeakTable<UIElement, UIElementObservables> UIElementCache = new();

	private static UIElementObservables CreateObservables( UIElement uiElement )
	{
		return new UIElementObservables( uiElement );
	}

	private static UIElementObservables GetObservables( this UIElement uiElement )
	{
		return UIElementCache.GetValue( uiElement, CreateObservables );
	}

	public static IObservable<EventPattern<object>> OnLayoutUpdated( this UIElement uiElement )
	{
		return uiElement.GetObservables().LayoutUpdated;
	}
}