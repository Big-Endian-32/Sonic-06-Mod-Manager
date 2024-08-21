#include "stdafx.h"

std::vector<HookDefinition*> HookService::m_definitions;

void HookService::LoadDefinitions()
{
	auto ini = INIReader::Read("game:\\SonicNextModLoader.ini");

	if (!ini.count("Hooks"))
		return;

	for (std::map<std::string, std::string>::iterator it = ini["Hooks"].begin(); it != ini["Hooks"].end(); it++)
	{
		auto addrStr = it->first;

		// Omit "0x" prefix.
		if (addrStr.substr(0, 2) == "0x")
			addrStr = addrStr.substr(2);

		auto addr = (size_t)std::stoul(addrStr, nullptr, 16);
		auto tupl = StringHelper::TransformHexStringToByteArray(it->second.c_str());

		m_definitions.push_back(new HookDefinition(addr, std::get<0>(tupl), std::get<1>(tupl)));
	}
}

void HookService::InstallHooks()
{
	for (std::vector<HookDefinition*>::iterator it = m_definitions.begin(); it != m_definitions.end(); it++)
	{
		HookDefinition* def = *it;

		auto len = def->Length + m_hookLength;
		auto mem = _aligned_malloc(len, 4);
		auto pos = (size_t)mem;

		// Change page protection for code execution.
		DWORD oldProtect;
		VirtualProtect(mem, len, PAGE_EXECUTE_READWRITE, &oldProtect);

		// Write hook code.
		memcpy_s(mem, len, def->pBuffer, len);

		// Write jump back to code.
		WRITE_JUMP_FAR(pos + len - m_hookLength, def->pAddr + m_hookLength);

		// Write jump to hook.
		WRITE_JUMP_FAR(def->pAddr, pos);
	}
}

void HookService::Init()
{
	LoadDefinitions();
	InstallHooks();
}