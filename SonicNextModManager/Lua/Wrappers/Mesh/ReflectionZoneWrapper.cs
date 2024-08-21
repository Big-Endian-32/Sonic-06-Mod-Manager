using Marathon.Formats.Archive;
using Marathon.Formats.Mesh;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Extensions;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Mesh
{
    [LuaUserData]
    public class ReflectionZoneWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private ReflectionZone _reflectionZone;

        public ReflectionZoneWrapper() { }

        public ReflectionZoneWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _reflectionZone = IOHelper.LoadMarathonTypeFromBuffer<ReflectionZone>(File.Data);
        }

        public ReflectionArea this[int in_index]
        {
            get => GetReflectionArea(in_index);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<ReflectionArea>();
        }

        public void AddReflectionArea(DynValue in_value)
        {
            _reflectionZone.Reflections.Add(in_value.ParseClassFromDynValue<ReflectionArea>());
        }

        public ReflectionArea GetReflectionArea(int in_index)
        {
            return _reflectionZone.Reflections[in_index];
        }

        public ReflectionArea[] GetReflectionAreas()
        {
            return [.. _reflectionZone.Reflections];
        }

        public void Close()
        {
            Close(_reflectionZone);
        }
    }
}
