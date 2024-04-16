using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using ManicBox.Interop.Exceptions;
using ManicBox.Interop.Extensions;

namespace ManicBox.Interop;

internal sealed class EventHookBuilder
{
	private const User32.WinEventFlags WIN_EVENT_FLAGS =
		User32.WinEventFlags.OutOfContext | User32.WinEventFlags.SkipOwnProcess;

	private static nint _hmodWinEventProc = nint.Zero;

	private User32.WinEventProc? _winEventProc;
	private User32.WinEvent _winEventMin;
	private User32.WinEvent _winEventMax;
	private uint _processId;
	private uint _threadId;

	public static EventHookBuilder OnEvent( User32.WinEvent winEvent )
	{
		return new EventHookBuilder
		{
			_winEventMin = winEvent,
			_winEventMax = winEvent,
		};
	}

	public static EventHookBuilder OnEvent( User32.WinEvent winEventMin, User32.WinEvent winEventMax )
	{
		return new EventHookBuilder
		{
			_winEventMin = winEventMin,
			_winEventMax = winEventMax,
		};
	}

	public EventHookBuilder ForProcess( uint processId, uint threadId )
	{
		this._processId = processId;
		this._threadId = threadId;
		return this;
	}

	public EventHookBuilder WithCallback( User32.WinEventProc winEventProc )
	{
		this._winEventProc = winEventProc;
		return this;
	}

	public IDisposable Subscribe()
	{
		var onDispose = new CompositeDisposable();

		var eventHook = User32.SetWinEventHook(
			_winEventMin,
			_winEventMax,
			_hmodWinEventProc,
			_winEventProc!,
			_processId,
			_threadId,
			WIN_EVENT_FLAGS
		);

		if ( eventHook == nint.Zero )
		{
			MarshalUtil.ThrowLastError();
		}

		Disposable.Create( () => User32.UnhookWinEvent( eventHook ) )
			.DisposeWith( onDispose );

		GCHandle.Alloc( _winEventProc )
			.AsDisposable( handle => handle.Free() )
			.DisposeWith( onDispose );

		return onDispose;
	}
}