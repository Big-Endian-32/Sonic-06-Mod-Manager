#pragma once

#include <string>

class StringHelper
{
public:
	static bool ContainsSubstring(const std::string in_str, const std::string in_substr)
	{
		return in_str.find(in_substr) != std::string::npos;
	}

	static std::string Trim(const std::string in_str, char in_char = ' ')
	{
		auto start = in_str.find_first_not_of(in_char);

		if (start == std::string::npos)
			return "";

		auto end = in_str.find_last_not_of(in_char);

		return in_str.substr(start, end - start + 1);
	}
	
	static bool IsEmpty(const char* in_pStr)
	{
		if (in_pStr && !in_pStr[0])
			return true;

		return false;
	}

	static std::tuple<uint8_t*, size_t> TransformHexStringToByteArray(const char* in_hexStr)
	{
		if (in_hexStr == nullptr)
			return std::make_tuple(nullptr, 0);

		auto len = strlen(in_hexStr);

		if ((len % 2) != 0)
			return std::make_tuple(nullptr, 0);

		auto bufferLen = len / 2;
		auto buffer = (uint8_t*)malloc(bufferLen);

		memset(buffer, 0, bufferLen);

		size_t index = 0;

		while (index < len)
		{
			char c = in_hexStr[index];
			int value = 0;

			if (c >= '0' && c <= '9')
			{
				value = (c - '0');
			}
			else if (c >= 'A' && c <= 'F')
			{
				value = (10 + (c - 'A'));
			}
			else if (c >= 'a' && c <= 'f')
			{
				value = (10 + (c - 'a'));
			}
			else
			{
				return std::make_tuple(nullptr, 0);
			}

			buffer[(index / 2)] += value << (((index + 1) % 2) * 4);

			index++;
		}

		return std::make_tuple(buffer, bufferLen);
	}

	static std::string GetSubstringBeforeLastChar(const std::string in_str, char in_char, int in_charIndex = 0)
	{
		std::string result;

		const size_t index = in_str.rfind(in_char);

		if (std::string::npos != index)
			result = in_str.substr(0, index);

		for (int i = 0; i < in_charIndex; i++)
			return GetSubstringBeforeLastChar(result, in_char, i);

		return result;
	}

	static std::string TransformWideCharToString(const wchar_t* in_pStr)
	{
		int length = WideCharToMultiByte(CP_ACP, 0, in_pStr, -1, NULL, 0, NULL, NULL);

		char* charArray = new char[length];

		WideCharToMultiByte(CP_ACP, 0, in_pStr, -1, charArray, length, NULL, NULL);

		std::string result(charArray);

		delete[] charArray;

		return result;
	}

	static wchar_t* TransformStringToWideChar(const std::string in_str)
	{
		int length = ::MultiByteToWideChar(CP_UTF8, 0, in_str.c_str(), -1, NULL, 0);

		wchar_t* buffer = new wchar_t[length];

		::MultiByteToWideChar(CP_UTF8, 0, in_str.c_str(), -1, buffer, length);

		return buffer;
	}
};