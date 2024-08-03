using SonicNextModManager.Metadata;

namespace SonicNextModManager.Interop
{
    public class PlatformConverter
    {
        /// <summary>
        /// Converts the platform from older metadata.
        /// </summary>
        /// <param name="platform">Platform by name.</param>
        public static Platform Convert(string platform)
        {
            if (!Enum.TryParse(platform, out Platform out_platform))
            {
                out_platform = platform switch
                {
                    "Xbox 360"      => Platform.Xbox,
                    "PlayStation 3" => Platform.PlayStation,
                    _               => Platform.Any,
                };
            }

            return out_platform;
        }
    }
}
