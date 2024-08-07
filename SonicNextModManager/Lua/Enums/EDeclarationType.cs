namespace SonicNextModManager.Lua
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EDeclarationType
    {
        Boolean,
        Integer,
        Float,
        String
    }
}
