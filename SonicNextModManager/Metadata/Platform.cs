namespace SonicNextModManager
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Platform
    {
        Any,
        Xbox,
        PlayStation
    }
}
