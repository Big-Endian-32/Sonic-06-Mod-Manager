using Marathon.Formats.Archive;
using Marathon.Formats.Placement;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Interfaces;

namespace SonicNextModManager.Lua.Wrappers.Placement
{
    [LuaUserData]
    public class SetDataWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private SetData _setData;

        public SetDataWrapper() { }

        public SetDataWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _setData = IOHelper.LoadMarathonTypeFromBuffer<SetData>(File.Data);
        }

        public SetObject this[string in_name]
        {
            get => GetObject(in_name);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<SetObject>();
            L.RegisterType<SetParameter>();
            L.RegisterType<SetGroup>();
            L.RegisterType<SetDataType>("ESetDataType");
        }

        public SetObject GetObject(string in_name)
        {
            return _setData.Data.Objects.Where(x => x.Name == in_name).FirstOrDefault()!;
        }

        public SetObject[] GetObjects()
        {
            return [.. _setData.Data.Objects];
        }

        public void SetObject(DynValue in_value)
        {
            var @object = in_value.ParseClassFromDynValue<SetObject>();
            var index = _setData.Data.Objects.FindIndex(x => x.Name == @object.Name);

            if (index == -1)
            {
                _setData.Data.Objects.Add(@object);
            }
            else
            {
                _setData.Data.Objects[index] = @object;
            }
        }

        public void Group(string in_name, string in_callback, params DynValue[] in_values)
        {
            var group = new SetGroup(in_name, in_callback);

            void GetIndices(DynValue[] in_values)
            {
                foreach (var value in in_values)
                {
                    // Aggregate objects inside of arrays.
                    if (value.IsArray())
                    {
                        GetIndices([.. value.Table.Values]);
                        continue;
                    }

                    var @object = value.ParseClassFromDynValue<SetObject>();
                    var index = _setData.Data.Objects.FindIndex(x => x.Name == @object.Name);

                    if (index == -1)
                        continue;

                    group.Objects.Add((ulong)index);
                }
            }

            GetIndices(in_values);

            _setData.Data.Groups.Add(group);
        }

        public void Save()
        {
            Save(_setData);
        }
    }
}
