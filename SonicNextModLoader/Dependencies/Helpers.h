#include "stdafx.h"

#define FUNCTION_PTR(in_returnType, in_callingConvention, in_functionName, in_location, ...) \
	in_returnType (in_callingConvention *in_functionName)(__VA_ARGS__) = (in_returnType(in_callingConvention*)(__VA_ARGS__))(in_location)

#define WRITE_MEMORY(in_location, in_type, ...) \
{ \
    if (in_location != 0) \
    { \
        const in_type data[] = { __VA_ARGS__ }; \
        DWORD oldProtect; \
        VirtualProtect((void*)(in_location), sizeof(data), PAGE_EXECUTE_READWRITE, &oldProtect); \
        memcpy((void*)(in_location), data, sizeof(data)); \
        VirtualProtect((void*)(in_location), sizeof(data), oldProtect, &oldProtect); \
    } \
}

#define WRITE_JUMP(in_source, in_destination) \
{ \
	if (in_source != 0 && in_destination != 0) \
	{ \
		WRITE_MEMORY(in_source, uint32_t, (0x48000000 + (((size_t)in_destination - (size_t)in_source) & 0x3FFFFFF))); \
	} \
}

#define WRITE_JUMP_FAR(in_source, in_destination) \
{ \
	if (in_source != 0 && in_destination != 0) \
	{ \
		WRITE_MEMORY(in_source + 0x00, uint32_t, 0x3C000000 + (in_destination >> 16));    /* lis   %r0, in_destination >> 16    */ \
		WRITE_MEMORY(in_source + 0x04, uint32_t, 0x60000000 + (in_destination & 0xFFFF)); /* ori   %r0, in_destination & 0xFFFF */ \
		WRITE_MEMORY(in_source + 0x08, uint32_t, 0x7C0903A6);                             /* mtctr %r0                          */ \
		WRITE_MEMORY(in_source + 0x0C, uint32_t, 0x4E800420);                             /* bctr                               */ \
	} \
}

#define WRITE_NOP(in_location, in_count) \
{ \
    if (in_location != 0) \
    { \
		for (size_t i = 0; i < (size_t)in_count; i++) \
			WRITE_MEMORY(in_location + i * 4, uint32_t, 0x60000000); \
    } \
}

inline static std::tuple<DWORD, MESSAGEBOX_RESULT> MessageBox
(
	LPCWSTR in_text,
	LPCWSTR in_caption = nullptr,
	DWORD in_flags = 0,
	LPCWSTR* in_buttons = nullptr,
	int32_t in_numButtons = 0,
	int32_t in_defaultButtonIndex = 0
)
{
	if (!in_caption)
		in_caption = L"Message";

	auto caption = std::wstring(in_caption);
	auto text    = std::wstring(in_text);

	/* SDK documentation suggests the title has a limit of 32
	   and the body a limit of 256, but this is incorrect. */

	if (wcslen(in_caption) > 43)
		caption = caption.substr(0, 39) + L"...";

	if (wcslen(in_text) > 384)
		text = text.substr(0, 380) + L"...";

	LPCWSTR defaultButton = L"OK";

	MESSAGEBOX_RESULT result;
	XOVERLAPPED overlapped;

	if (!in_buttons)
	{
		in_buttons = &defaultButton;
		in_numButtons = 1;
	}

	auto error = XShowMessageBoxUI(0, caption.c_str(), text.c_str(), in_numButtons, in_buttons, in_defaultButtonIndex, in_flags, &result, &overlapped);

	return std::tuple<DWORD, MESSAGEBOX_RESULT>(error, result);
}

inline static std::tuple<DWORD, MESSAGEBOX_RESULT> MessageBoxF(const char* in_text, ...)
{
	va_list va;
	va_start(va, in_text);

	// Create formatted string.
	char buffer[512];
	_vsprintf_p(buffer, sizeof(buffer), in_text, va);

	// Transform to wide string.
	int length = ::MultiByteToWideChar(CP_UTF8, 0, buffer, -1, NULL, 0);
	wchar_t* wbuffer = new wchar_t[length];
	::MultiByteToWideChar(CP_UTF8, 0, buffer, -1, wbuffer, length);

	auto result = MessageBox(wbuffer);

	va_end(va);

	return result;
}