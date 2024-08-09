using Marathon.Formats.Archive;
using Marathon.Formats.Text;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Text
{
    [LuaUserData]
    public class MessageTableWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private MessageTable _messageTable;

        public MessageTableWrapper() { }

        public MessageTableWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _messageTable = IOHelper.LoadMarathonTypeFromBuffer<MessageTable>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<Message>();
        }

        public Message GetMessage(string in_name)
        {
            return _messageTable.Data.Messages.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public Message[] GetMessages()
        {
            return [.. _messageTable.Data.Messages];
        }

        public void SetMessage(string in_name, string in_text, params string[] in_placeholders)
        {
            if (_messageTable.Data.Messages.Any(x => x.Name == in_name))
            {
                var msg = GetMessage(in_name);

                msg.Text = in_text;
                msg.Placeholders = in_placeholders;
            }
            else
            {
                _messageTable.Data.Messages.Add(new(in_name, in_text, in_placeholders));
            }
        }

        public void Save()
        {
            Save(_messageTable);
        }
    }
}
