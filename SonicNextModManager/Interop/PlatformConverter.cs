using SonicNextModManager.Metadata;

namespace SonicNextModManager.Interop
{
    public class PlatformConverter
    {
        /// <summary>
        /// Converts the platform from older metadata.
        /// </summary>
        /// <param name="in_platform">Platform by name.</param>
        public static EPlatform Convert(string in_platform)
        {
            if (!Enum.TryParse(in_platform, out EPlatform out_platform))
            {
                out_platform = in_platform switch
                {
                    "Xbox 360"      => EPlatform.Xbox,
                    "PlayStation 3" => EPlatform.PlayStation,
                    _               => EPlatform.Any,
                };
            }

            return out_platform;
        }
    }
}
