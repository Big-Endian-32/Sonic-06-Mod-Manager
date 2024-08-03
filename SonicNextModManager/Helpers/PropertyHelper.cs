namespace SonicNextModManager.Helpers
{
    public class PropertyHelper
    {
        /// <summary>
        /// Sets a string property to a new value if the new value isn't null or empty.
        /// </summary>
        /// <param name="setter">Setter expression.</param>
        /// <param name="value">Value to set to.</param>
        public static void SetStringWithNullCheck(Action<string> setter, string value)
        {
            if (!string.IsNullOrEmpty(value))
                setter(value);
        }
    }
}
