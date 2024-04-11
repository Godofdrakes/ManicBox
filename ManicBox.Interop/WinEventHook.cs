using System.Reactive.Disposables;
using ManicBox.Interop.Exceptions;
using ManicBox.Interop.Extensions;

namespace ManicBox.Interop;

internal sealed class WinEventHook : IDisposable
{
	private readonly CompositeDisposable _onDispose = new();

	// ReSharper disable once NotAccessedField.Local
	private readonly User32.WinEventProc _callback;

	public WinEventHook( User32.WinEvent @event, User32.WinEventProc callback )
	{
		ArgumentNullException.ThrowIfNull( callback );

		// Keep a reference to the callback so it doesn't get GCd
		_callback = callback;

		SetWinEventHook( @event, @event, 0, 0, callback )
			.DisposeWith( _onDispose );
	}

	public WinEventHook( User32.WinEvent @event, uint idProcess, uint idThread, User32.WinEventProc callback )
	{
		ArgumentNullException.ThrowIfNull( callback );

		// Keep a reference to the callback so it doesn't get GCd
		_callback = callback;

		SetWinEventHook( @event, @event, idProcess, idThread, callback )
			.DisposeWith( _onDispose );
	}

	public WinEventHook( User32.WinEvent eventMin, User32.WinEvent eventMax, User32.WinEventProc callback )
	{
		ArgumentNullException.ThrowIfNull( callback );

		// Keep a reference to the callback so it doesn't get GCd
		_callback = callback;

		SetWinEventHook( eventMin, eventMax, 0, 0, callback )
			.DisposeWith( _onDispose );
	}

	public WinEventHook(
		User32.WinEvent eventMin,
		User32.WinEvent eventMax,
		uint idProcess,
		uint idThread,
		User32.WinEventProc callback )
	{
		ArgumentNullException.ThrowIfNull( callback );

		// Keep a reference to the callback so it doesn't get GCd
		_callback = callback;

		SetWinEventHook( eventMin, eventMax, idProcess, idThread, callback )
			.DisposeWith( _onDispose );
	}

	private static IDisposable SetWinEventHook(
		User32.WinEvent eventMin,
		User32.WinEvent eventMax,
		uint idProcess,
		uint idThread,
		User32.WinEventProc callback )
	{
		var eventHook = User32.SetWinEventHook(
			eventMin,
			eventMax,
			nint.Zero,
			callback,
			idProcess,
			idThread,
			User32.WinEventFlags.OutOfContext | User32.WinEventFlags.SkipOwnProcess
		);

		if (eventHook == nint.Zero)
		{
			HResult.ThrowLastPInvokeError();
		}

		return Disposable.Create( () => User32.UnhookWinEvent( eventHook ) );
	}

	public void Dispose() => _onDispose.Dispose();
}