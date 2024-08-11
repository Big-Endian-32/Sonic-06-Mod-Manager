using Marathon.Formats.Archive;
using Marathon.Formats.Event;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Event
{
    [LuaUserData]
    public class TimeEventWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private TimeEvent _timeEvent;

        public TimeEventWrapper() { }

        public TimeEventWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _timeEvent = IOHelper.LoadMarathonTypeFromBuffer<TimeEvent>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<AnimationTimer>();
        }

        public void AddEvent(DynValue in_value)
        {
            _timeEvent.Data.Events.Add(in_value.ParseClassFromDynValue<AnimationTimer>());
        }

        public AnimationTimer[] GetEvents()
        {
            return [.. _timeEvent.Data.Events];
        }

        public string GetAnimation()
        {
            return _timeEvent.Data.Animation;
        }

        public void SetAnimation(string in_path)
        {
            _timeEvent.Data.Animation = in_path;
        }

        public void Save()
        {
            Save(_timeEvent);
        }
    }
}
