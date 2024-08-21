#pragma once

struct HookDefinition
{
	size_t pAddr;
	uint8_t* pBuffer;
	size_t Length;

	HookDefinition() : pAddr(0), pBuffer(nullptr), Length(0) { }

	HookDefinition(size_t in_pAddr, uint8_t* in_pBuffer, size_t in_length)
		: pAddr(in_pAddr), pBuffer(in_pBuffer), Length(in_length) { }
};

class HookService
{
private:
	static const size_t m_hookLength = 16;

	static std::vector<HookDefinition*> m_definitions;

public:
	static void LoadDefinitions();
	static void InstallHooks();
	static void Init();
};

