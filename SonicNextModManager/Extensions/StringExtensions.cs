namespace SonicNextModManager.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Splits the input string by both CRLF and LF line endings.
        /// </summary>
        /// <param name="in_str">The string to split.</param>
        public static string[] GetLines(this string in_str)
        {
            return in_str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        /// <summary>
        /// Indents each line in a string.
        /// </summary>
        /// <param name="in_str">The string containing lines to indent.</param>
        /// <param name="in_isTabs">Determines whether to use tabs for indentation.</param>
        public static string Indent(this string in_str, int in_amount, bool in_isTabs = false)
        {
            var lines = in_str.GetLines();

            for (int i = 0; i < lines.Length; i++)
                lines[i] = new string(in_isTabs ? '\t' : ' ', in_isTabs ? in_amount : in_amount * 4) + lines[i];

            return string.Join('\n', lines);
        }

        /// <summary>
        /// Gets the lowest amount of indentation used by the string.
        /// </summary>
        /// <param name="in_str">The string to get indentation from.</param>
        public static int GetIndentation(this string in_str)
        {
            var result = 0;
            var lines = in_str.GetLines();

            for (int i = 0; i < lines.Length; i++)
                result = lines[i].Length - lines[i].TrimStart().Length;

            return result;
        }
    }
}
