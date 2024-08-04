namespace SonicNextModManager.Lua
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeclarationType
    {
        Boolean,
        Integer,
        Float,
        String
    }
}
