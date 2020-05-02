// GlobalCbtHook.cpp
//   by Chris Wilson
//	 modded by Michael Jauregui
//	 for Autumn Shell Project

#include "stdafx.h"
#include <windows.h>
#include "GlobalCbtHook.h"

extern "C" VOID __cdecl SetSharedMem(HWND lpszBuf, int pos);
extern "C" HWND __cdecl GetSharedMem( int pos );

HHOOK hookShell = NULL;

//
// Store the application instance of this module to pass to
// hook initialization. This is set in DLLMain().
//
HINSTANCE g_appInstance = NULL;

typedef void (CALLBACK *HookProc)(int code, WPARAM w, LPARAM l);

static LRESULT CALLBACK ShellHookCallback(int code, WPARAM wparam, LPARAM lparam);


bool InitializeShellHook(int threadID, HWND destination)
{
	if (g_appInstance == NULL)
	{
		return false;
	}
	
	SetSharedMem(destination,1);

	hookShell = SetWindowsHookEx(WH_SHELL, (HOOKPROC)ShellHookCallback, g_appInstance, threadID);
	return hookShell != NULL;
}

void UninitializeShellHook()
{
	if (hookShell != NULL)
		UnhookWindowsHookEx(hookShell);
	hookShell = NULL;
}

static LRESULT CALLBACK ShellHookCallback(int code, WPARAM wparam, LPARAM lparam)
{
	if (code >= 0)
	{
		UINT msg = 0;

		if (code == HSHELL_ACTIVATESHELLWINDOW)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_ACTIVATESHELLWINDOW");
		else if (code == HSHELL_GETMINRECT)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_GETMINRECT");
		else if (code == HSHELL_LANGUAGE)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_LANGUAGE");
		else if (code == HSHELL_REDRAW)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_REDRAW");
		else if (code == HSHELL_TASKMAN)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_TASKMAN");
		else if (code == HSHELL_WINDOWACTIVATED)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_WINDOWACTIVATED");
		else if (code == HSHELL_WINDOWCREATED)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_WINDOWCREATED");
		else if (code == HSHELL_WINDOWDESTROYED)
			msg = RegisterWindowMessage("AUTUMN_HSHELL_WINDOWDESTROYED");

		HWND dstWnd = GetSharedMem(1);

		if (msg != 0)
			SendNotifyMessage(dstWnd, msg, wparam, lparam);
	}

	return CallNextHookEx(hookShell, code, wparam, lparam);
}