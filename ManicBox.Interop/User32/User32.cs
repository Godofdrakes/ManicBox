﻿using System.Reactive.Linq;
using ManicBox.Interop.Exceptions;

namespace ManicBox.Interop;

public static partial class User32
{
	// Observe changes in window focus
	public static IObservable<HWND> ForegroundWindowChanged()
	{
		return EventHook( WinEvent.SystemForeground )
			.Select( ev => ev.hWnd )
			.StartWith( GetForegroundWindow() )
			.Where( IsShellWindow( false ) );
	}

	// Observe changes in a window's title
	public static IObservable<string> WindowTitleChanged( HWND hWnd )
	{
		var idThread = GetWindowThreadProcessId( hWnd, out var idProcess );

		if ( idThread == 0 )
		{
			MarshalUtil.ThrowLastError();
		}

		return EventHook( WinEvent.ObjectNameChange, idProcess, idThread )
			.Where( e => e.idObject == OBJID_WINDOW )
			.Where( e => e.idChild == CHILDID_SELF )
			.Select( e => e.hWnd )
			.StartWith( hWnd )
			.Select( GetWindowTitle );
	}

	private static Func<HWND, bool> IsShellWindow( bool expected )
	{
		var shellThread = GetWindowThreadProcessId( GetShellWindow(), out var shellProcess );

		if ( shellThread == 0 )
		{
			MarshalUtil.ThrowLastError();
		}

		return window =>
		{
			var windowThread = GetWindowThreadProcessId( window, out var windowProcess );

			if ( windowThread == 0 )
			{
				MarshalUtil.ThrowLastError();
			}

			return (windowProcess == shellProcess) == expected;
		};
	}
}