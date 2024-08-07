namespace SonicNextModManager.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EPlatform
    {
        Any,
        Xbox,
        PlayStation
    }
}
