using SonicNextModManager.Metadata;

namespace SonicNextModManager.Interop
{
    public class PlatformConverter
    {
        /// <summary>
        /// Converts the platform from older metadata.
        /// </summary>
        /// <param name="in_platform">Platform by name.</param>
        public static Platform Convert(string in_platform)
        {
            if (!Enum.TryParse(in_platform, out Platform out_platform))
            {
                out_platform = in_platform switch
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
