using Marathon.Formats.Archive;
using Marathon.Formats.Event;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

using _Event = Marathon.Formats.Event.Event;

namespace SonicNextModManager.Lua.Wrappers.Event
{
    [LuaUserData]
    public class EventPlaybookWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private EventPlaybook _eventPlaybook;

        public EventPlaybookWrapper() { }

        public EventPlaybookWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _eventPlaybook = IOHelper.LoadMarathonTypeFromBuffer<EventPlaybook>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<_Event>();
        }

        public _Event GetEvent(string in_name)
        {
            return _eventPlaybook.Events.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public _Event[] GetEvents()
        {
            return [.. _eventPlaybook.Events];
        }

        public void SetEvent(DynValue in_table)
        {
            var @event = in_table.ParseClassFromDynValue<_Event>();
            var index = _eventPlaybook.Events.FindIndex(x => x.Name == @event.Name);

            if (index == -1)
            {
                _eventPlaybook.Events.Add(@event);
            }
            else
            {
                _eventPlaybook.Events[index] = @event;
            }
        }

        public void Save()
        {
            Save(_eventPlaybook);
        }
    }
}
