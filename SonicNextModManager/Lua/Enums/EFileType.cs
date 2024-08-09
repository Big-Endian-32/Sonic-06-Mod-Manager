using SonicNextModManager.Lua.Attributes;

namespace SonicNextModManager.Lua
{
    [LuaEnum]
    public enum EFileType
    {
        Guess,
        Buffer,
        SoundBank,
        EventPlaybook,
        TimeEvent,
        Collision,
        PathSpline,
        ReflectionZone,
        AssetPackage,
        CommonPackage,
        ExplosionPackage,
        PathPackage,
        ScriptPackage,
        ShotPackage,
        ParticleContainer,
        ParticleEffectBank,
        ParticleGenerationSystem,
        ParticleTextureBank,
        ObjectPlacement,
        ObjectPropertyDatabase,
        SonicNextSaveData,
        LuaBinary,
        MessageTable,
        PictureFont
    }
}
