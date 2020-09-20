#include "logger.h"
#include <cstdio>
#include <Windows.h>

HANDLE h_out = nullptr, h_old_out = nullptr;
HANDLE h_err = nullptr, h_old_err = nullptr;
HANDLE h_in = nullptr, h_old_in = nullptr;

namespace Logger
{
	void Attach()
	{
		h_old_out = GetStdHandle(STD_OUTPUT_HANDLE);
		h_old_err = GetStdHandle(STD_ERROR_HANDLE);
		h_old_in = GetStdHandle(STD_INPUT_HANDLE);

		AllocConsole() && AttachConsole(GetCurrentProcessId());

		h_out = GetStdHandle(STD_OUTPUT_HANDLE);
		h_err = GetStdHandle(STD_ERROR_HANDLE);
		h_in = GetStdHandle(STD_INPUT_HANDLE);

		SetConsoleMode(h_out,
			ENABLE_PROCESSED_OUTPUT | ENABLE_WRAP_AT_EOL_OUTPUT);

		SetConsoleMode(h_in,
			ENABLE_INSERT_MODE | ENABLE_EXTENDED_FLAGS |
			ENABLE_PROCESSED_INPUT | ENABLE_QUICK_EDIT_MODE);
	}

	void Detach()
	{
		if (h_out && h_err && h_in) {
			FreeConsole();

			if (h_old_out)
			{
				SetStdHandle(STD_OUTPUT_HANDLE, h_old_out);
			}

			if (h_old_err)
			{
				SetStdHandle(STD_ERROR_HANDLE, h_old_err);
			}

			if (h_old_in)
			{
				SetStdHandle(STD_INPUT_HANDLE, h_old_in);
			}
		}
	}

	bool Print(const char* fmt, ...)
	{
		if (!h_out)
		{
			return false;
		}

		char buf[1024];
		va_list va;

		va_start(va, fmt);
		_vsnprintf_s(buf, 1024, fmt, va);
		va_end(va);

		return !!WriteConsoleA(h_out, buf, static_cast<DWORD>(strlen(buf)), nullptr, nullptr);
	}

	char ReadKey()
	{
		if (!h_in)
		{
			return -1;
		}

		auto key = char{ 0 };
		auto keysread = DWORD{ 0 };

		ReadConsoleA(h_in, &key, 1, &keysread, nullptr);

		return key;
	}
};