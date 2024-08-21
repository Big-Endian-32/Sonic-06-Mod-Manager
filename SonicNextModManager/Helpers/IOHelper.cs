using Marathon.IO;

namespace SonicNextModManager.Helpers
{
    public class IOHelper
    {
        /// <summary>
        /// Loads a Marathon format type from a byte buffer.
        /// </summary>
        /// <typeparam name="T">The type to load.</typeparam>
        /// <param name="in_data">The buffer to read from.</param>
        public static T LoadMarathonTypeFromBuffer<T>(byte[] in_data) where T : FileBase, new()
        {
            if (in_data == null)
                return default;

            using var ms = new MemoryStream(in_data);

            T t = new();
            t.Load(ms);

            return t;
        }

        /// <summary>
        /// Gets the buffer from a Marathon format type.
        /// </summary>
        /// <typeparam name="T">The type to get the buffer from.</typeparam>
        /// <param name="in_instance">The instance to get the buffer from.</param>
        public static byte[] GetMarathonTypeBuffer<T>(T in_instance) where T : FileBase
        {
            using var ms = new MemoryStream();

            in_instance.Save(ms);

            return ms.ToArray();
        }
    }
}
