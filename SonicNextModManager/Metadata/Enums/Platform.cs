namespace SonicNextModManager.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Platform
    {
        Any,
        Xbox,
        PlayStation
    }
}
