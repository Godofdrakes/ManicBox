using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ManicBox.Common.Extensions;

public static class WindowExtensions
{
	private sealed class State : IDisposable
	{
		public IServiceScope ServiceScope { get; }

		public IServiceProvider ServiceProvider { get; }

		public State( IServiceScope serviceScope )
		{
			ArgumentNullException.ThrowIfNull( serviceScope );

			ServiceScope = serviceScope;
			ServiceProvider = serviceScope.ServiceProvider;
		}

		public void Dispose() => ServiceScope.Dispose();
	}

	private static readonly ConditionalWeakTable<Window, State> WindowState = new();

	private static void DisposeWindowState( object? sender, EventArgs eventArgs )
	{
		if ( sender is not Window window )
		{
			throw new InvalidOperationException();
		}

		if ( !WindowState.TryGetValue( window, out State? state ) )
		{
			throw new InvalidOperationException();
		}

		state.Dispose();
	}

	public static TWindow CreateWindow<TWindow>( this IServiceProvider serviceProvider )
		where TWindow : Window
	{
		ArgumentNullException.ThrowIfNull( serviceProvider );

		IServiceScope serviceScope = serviceProvider.CreateScope();

		var state = new State( serviceScope );

		var window = ActivatorUtilities.CreateInstance<TWindow>( serviceProvider );

		WindowState.Add( window, state );

		window.Closed += DisposeWindowState;

		return window;
	}

	public static TWindow CreateWindow<TWindow>( this IServiceProvider serviceProvider, Action<TWindow> action )
		where TWindow : Window
	{
		ArgumentNullException.ThrowIfNull( action );

		var window = CreateWindow<TWindow>( serviceProvider );

		action( window );

		return window;
	}

	public static IServiceProvider GetServiceProvider( this Window window )
	{
		ArgumentNullException.ThrowIfNull( window );

		if ( !WindowState.TryGetValue( window, out State? state ) )
		{
			throw new InvalidOperationException();
		}

		return state.ServiceProvider;
	}
}