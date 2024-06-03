using ReactiveUI;

namespace ManicBox.Common.Services;

public sealed class ScreenRoutingService : IScreen
{
	public RoutingState Router { get; } = new();
}