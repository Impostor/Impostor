#include <Windows.h>
#include "logger.h"

DWORD WINAPI OnDllAttach(LPVOID base)
{
	Logger::Attach();
	Logger::Print("[AmongUs.Agent] DLL Attached\n");

	FreeLibraryAndExitThread(static_cast<HMODULE>(base), 1);
}

BOOL WINAPI OnDllDetach()
{
	Logger::Print("[AmongUs.Agent] DLL Detached\n");
	Logger::Detach();

	return true;
}

BOOL WINAPI DllMain(
	const _In_      HINSTANCE hinstDll,
	const _In_      DWORD fdwReason,
	const _In_opt_  LPVOID lpvReserved
)
{
	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hinstDll);
		CreateThread(nullptr, 0, OnDllAttach, hinstDll, 0, nullptr);
		return true;

	case DLL_PROCESS_DETACH:
		if (lpvReserved == nullptr)
			return OnDllDetach();
		return true;
	default:
		return true;
	}
}