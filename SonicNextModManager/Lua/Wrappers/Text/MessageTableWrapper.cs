using Marathon.Formats.Archive;
using Marathon.Formats.Text;
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

        public void Register()
        {
            UserData.RegisterType<Message>();
        }

        public Message GetMessage(string in_name)
        {
            return _messageTable.Data.Messages.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public void SetMessage(string in_name, string in_text, params string[] in_placeholders)
        {
            var msg = GetMessage(in_name);

            msg.Text = in_text;
            msg.Placeholders = in_placeholders;
        }

        public void Save()
        {
            Save(_messageTable);
        }
    }
}
