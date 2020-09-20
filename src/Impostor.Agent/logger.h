#pragma once

namespace Logger
{
	void Attach();
	void Detach();
	bool Print(const char* fmt, ...);
	char ReadKey();
};