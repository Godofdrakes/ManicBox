using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static partial class User32
{
	internal enum WinEvent : uint
	{
		SystemForeground = 0x0003,
		ObjectNameChange = 0x800C,
	}

	[Flags]
	internal enum WinEventFlags : uint
	{
		OutOfContext = 0,
		SkipOwnThread = 1,
		SkipOwnProcess = 2,
		InContext = 4,
	}

	internal delegate void WinEventProc(
		nint hWinEventHook,
		WinEvent eventType,
		HWND hwnd,
		int idObject,
		int idChild,
		uint idEventThread,
		uint dwmsEventTime );

	[SuppressMessage( "ReSharper", "InconsistentNaming" )]
	internal struct SetWinEventParams
	{
		public WinEventProc pfnWinEventProc;
		public WinEvent eventMin;
		public WinEvent eventMax;
		public uint idProcess;
		public uint idThread;
	}

	[SuppressMessage( "ReSharper", "InconsistentNaming" )]
	[SuppressMessage( "ReSharper", "IdentifierTypo" )]
	internal sealed class WinEventParams
	{
		public nint hWinEventHook { get; init; }
		public WinEvent eventType { get; init; }
		public HWND hWnd { get; init; }
		public int idObject { get; init; }
		public int idChild { get; init; }
		public uint idEventThread { get; init; }
		public uint dwmsEventTime { get; init; }
	}

	internal sealed class WinEventHandle : IDisposable
	{
		public WinEventHandle( SetWinEventParams eventParams )
		{
			this._eventParams = eventParams;

			this._handle = SetWinEventHook(
				eventParams.eventMin,
				eventParams.eventMax,
				nint.Zero,
				eventParams.pfnWinEventProc,
				eventParams.idProcess,
				eventParams.idThread,
				WinEventFlags.OutOfContext | WinEventFlags.SkipOwnProcess
			);

			if ( this._handle == nint.Zero )
			{
				MarshalUtil.ThrowLastError();
			}
		}

		// Need to hold on to the eventProc so it doesn't get GCd
		// ReSharper disable once NotAccessedField.Local
		private readonly SetWinEventParams _eventParams;

		private readonly nint _handle;

		public void Dispose()
		{
			if ( this._handle != nint.Zero )
			{
				UnhookWinEvent( _handle );
			}
		}
	}

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern nint SetWinEventHook(
		WinEvent eventMin,
		WinEvent eventMax,
		nint hmodWinEventProc,
		WinEventProc lpfnWinEventProc,
		uint idProcess,
		uint idThread,
		WinEventFlags dwflags );

	[DllImport( nameof(User32), SetLastError = true )]
	internal static extern bool UnhookWinEvent( nint hWinEventHook );

	internal static IObservable<WinEventParams> EventHook(
		WinEvent winEventMin,
		WinEvent winEventMax,
		uint idProcess = 0,
		uint idThread = 0 )
	{
		return Observable.Create<WinEventParams>( observer =>
		{
			SetWinEventParams eventParams;
			eventParams.pfnWinEventProc = WinEventProc;
			eventParams.eventMin = winEventMin;
			eventParams.eventMax = winEventMax;
			eventParams.idProcess = idProcess;
			eventParams.idThread = idThread;

			return new WinEventHandle( eventParams );

			void WinEventProc(
				nint hWinEventHook,
				WinEvent eventType,
				HWND hWnd,
				int idObject,
				int idChild,
				uint idEventThread,
				uint dwmsEventTime )
			{
				observer.OnNext( new WinEventParams
				{
					hWinEventHook = hWinEventHook,
					eventType = eventType,
					hWnd = hWnd,
					idObject = idObject,
					idEventThread = idEventThread,
					dwmsEventTime = dwmsEventTime
				} );
			}
		} );
	}

	internal static IObservable<WinEventParams> EventHook(
		WinEvent winEvent,
		uint idProcess = 0,
		uint idThread = 0 )
	{
		return EventHook( winEvent, winEvent, idProcess, idThread );
	}
}