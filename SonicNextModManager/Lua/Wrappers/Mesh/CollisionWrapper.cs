using Marathon.Formats.Archive;
using Marathon.Formats.Mesh;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Extensions;
using SonicNextModManager.Lua.Interfaces;
using System.Numerics;

namespace SonicNextModManager.Lua.Wrappers.Mesh
{
    [LuaUserData]
    public class CollisionWrapper : MarathonWrapper, ILuaUserDataDescriptor
    {
        private Collision _collision;

        public CollisionWrapper() { }

        public CollisionWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _collision = IOHelper.LoadMarathonTypeFromBuffer<Collision>(File.Data);
        }

        public void Register(MoonSharp.Interpreter.Script L)
        {
            L.RegisterType<CollisionFace>();
            L.RegisterType<CollisionFlag>("ECollisionFlag");
        }

        public Vector3[] GetVertices()
        {
            return [.. _collision.Data.Vertices];
        }

        public CollisionFace[] GetFaces()
        {
            return [.. _collision.Data.Faces];
        }

        public void Close()
        {
            Close(_collision);
        }
    }
}
