using Marathon.Formats.Archive;
using Marathon.Formats.Text;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Text
{
    [LuaUserData]
    public class PictureFontWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private PictureFont _pictureFont;

        public PictureFontWrapper() { }

        public PictureFontWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _pictureFont = IOHelper.LoadMarathonTypeFromBuffer<PictureFont>(File.Data);
        }

        public Picture this[string in_name]
        {
            get => GetPicture(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<Picture>();
        }

        public string GetTexture()
        {
            return _pictureFont.Data.Texture;
        }

        public void SetTexture(string in_path)
        {
            _pictureFont.Data.Texture = in_path;
        }

        public Picture GetPicture(string in_name)
        {
            return _pictureFont.Data.Entries.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public Picture[] GetPictures()
        {
            return [.. _pictureFont.Data.Entries];
        }

        public void SetPicture(string in_name, ushort in_x, ushort in_y, ushort in_width, ushort in_height)
        {
            if (_pictureFont.Data.Entries.Any(x => x.Name == in_name))
            {
                var picture = GetPicture(in_name);

                picture.X = in_x;
                picture.Y = in_y;
                picture.Width = in_width;
                picture.Height = in_height;
            }
            else
            {
                _pictureFont.Data.Entries.Add(new(in_name, in_x, in_y, in_width, in_height));
            }
        }

        public void SetPicture(DynValue in_value)
        {
            var picture = in_value.ParseClassFromDynValue<Picture>();
            var index = _pictureFont.Data.Entries.FindIndex(x => x.Name == picture.Name);

            if (index == -1)
            {
                _pictureFont.Data.Entries.Add(picture);
            }
            else
            {
                _pictureFont.Data.Entries[index] = picture;
            }
        }

        public void Save()
        {
            Save(_pictureFont);
        }
    }
}
