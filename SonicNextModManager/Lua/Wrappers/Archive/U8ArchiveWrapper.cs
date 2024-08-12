using Marathon.Formats.Archive;
using Marathon.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Wrappers.Audio;
using SonicNextModManager.Lua.Wrappers.Event;
using SonicNextModManager.Lua.Wrappers.Mesh;
using SonicNextModManager.Lua.Wrappers.Package;
using SonicNextModManager.Lua.Wrappers.Placement;
using SonicNextModManager.Lua.Wrappers.Script;
using SonicNextModManager.Lua.Wrappers.Text;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Lua.Wrappers.Archive
{
    [LuaUserData]
    public class U8ArchiveWrapper
    {
        private U8Archive? _archive;

        public U8ArchiveWrapper() { }

        public U8ArchiveWrapper(string in_path)
        {
            _archive = Database.LoadArchive(in_path);
        }

        public DynValue this[string in_path, EFileType in_type = EFileType.Guess]
        {
            get => Load(in_path, in_type);
        }

        public DynValue Load(string in_path, EFileType in_type = EFileType.Guess)
        {
            // Change ".lua" to ".lub" to check if the file exists.
            if (Path.GetExtension(in_path) == ".lua")
                in_path = Path.ChangeExtension(in_path, ".lub");

            if (_archive?.Root.FileExists(in_path) == false)
                return DynValue.Nil;

            var fileName = Path.GetFileName(in_path);

            if (in_type != EFileType.Guess)
            {
                fileName = in_type switch
                {
                    EFileType.SoundBank                => Path.ChangeExtension(fileName, ".sbk"),
                    EFileType.EventPlaybook            => Path.ChangeExtension(fileName, ".epb"),
                    EFileType.TimeEvent                => Path.ChangeExtension(fileName, ".tev"),
                    EFileType.Collision                => "collision.bin",
                    EFileType.PathSpline               => Path.ChangeExtension(fileName, ".path"),
                    EFileType.ReflectionZone           => Path.ChangeExtension(fileName, ".rab"),
                    EFileType.AssetPackage             => Path.ChangeExtension(fileName, ".pkg"),
                    EFileType.CommonPackage            => "Common.bin",
                    EFileType.ExplosionPackage         => "Explosion.bin",
                    EFileType.PathPackage              => "PathObj.bin",
                    EFileType.ScriptPackage            => "ScriptParameter.bin",
                    EFileType.ShotPackage              => "ShotParameter.bin",
                    EFileType.ParticleContainer        => Path.ChangeExtension(fileName, ".plc"),
                    EFileType.ParticleEffectBank       => Path.ChangeExtension(fileName, ".peb"),
                    EFileType.ParticleGenerationSystem => Path.ChangeExtension(fileName, ".pgs"),
                    EFileType.ParticleTextureBank      => Path.ChangeExtension(fileName, ".ptb"),
                    EFileType.ObjectPlacement          => Path.ChangeExtension(fileName, ".set"),
                    EFileType.ObjectPropertyDatabase   => Path.ChangeExtension(fileName, ".prop"),
                    EFileType.LuaBinary                => Path.ChangeExtension(fileName, ".lub"),
                    EFileType.MessageTable             => Path.ChangeExtension(fileName, ".mst"),
                    EFileType.PictureFont              => Path.ChangeExtension(fileName, ".pft")
                };

                in_type = EFileType.Guess;
            }

            if (in_type == EFileType.Guess)
            {
                switch (Path.GetExtension(fileName))
                {
                    // Common
                    case ".bin":
                    {
                        if (in_type == EFileType.Guess)
                        {
                            switch (fileName)
                            {
                                // Collision
                                case "collision.bin":
                                    return MarathonWrapper.RegisterWrapper<CollisionWrapper>(_archive, in_path);

                                // CommonPackage
                                case "Common.bin":
                                    return MarathonWrapper.RegisterWrapper<CommonPackageWrapper>(_archive, in_path);

                                // ExplosionPackage
                                case "Explosion.bin":
                                    return MarathonWrapper.RegisterWrapper<ExplosionPackageWrapper>(_archive, in_path);

                                // Path Package
                                case "PathObj.bin":
                                    return MarathonWrapper.RegisterWrapper<PathPackageWrapper>(_archive, in_path);

                                // ScriptPackage
                                case "ScriptParameter.bin":
                                    return MarathonWrapper.RegisterWrapper<ScriptPackageWrapper>(_archive, in_path);

                                // ShotPackage
                                case "ShotParameter.bin":
                                    return MarathonWrapper.RegisterWrapper<ShotPackageWrapper>(_archive, in_path);
                            }
                        }

                        break;
                    }

                    // EventPlaybook
                    case ".epb":
                        return MarathonWrapper.RegisterWrapper<EventPlaybookWrapper>(_archive, in_path);

                    // LuaBinary
                    case ".lua":
                    case ".lub":
                        return MarathonWrapper.RegisterWrapper<LuaBinaryWrapper>(_archive, in_path);

                    // MessageTable
                    case ".mst":
                        return MarathonWrapper.RegisterWrapper<MessageTableWrapper>(_archive, in_path);

                    // AssetPackage
                    case ".pkg":
                        return MarathonWrapper.RegisterWrapper<AssetPackageWrapper>(_archive, in_path);

                    // SoundBank
                    case ".sbk":
                        return MarathonWrapper.RegisterWrapper<SoundBankWrapper>(_archive, in_path);

                    // SetData
                    case ".set":
                        return MarathonWrapper.RegisterWrapper<SetDataWrapper>(_archive, in_path);

                    // TimeEvent
                    case ".tev":
                        return MarathonWrapper.RegisterWrapper<TimeEventWrapper>(_archive, in_path);

                    case ".path": // PathSpline
                    case ".rab":  // ReflectionZone
                    case ".plc":  // ParticleContainer
                    case ".peb":  // ParticleEffectBank
                    case ".pgs":  // ParticleGenerationSystem
                    case ".ptb":  // ParticleTextureBank
                    case ".prop": // PropertyDatabase
                    case ".pft":  // PictureFont
                        throw new NotImplementedException();
                }
            }

            // Return a buffer if no Marathon type could be evaluated.
            return UserData.Create(new BufferWrapper(_archive.Root.GetFile(in_path).Data));
        }
    }
}
