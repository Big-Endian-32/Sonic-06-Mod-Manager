using System.Text.RegularExpressions;

namespace SonicNextModManager.Lua.Syntax
{
    public class SanitisedSyntaxBase
    {
        public string InstallSanitised(string in_code, Func<string, string> in_callback)
        {
            var (code, placeholders) = PreserveUserData(in_code);

            code = in_callback(code);

            return RestoreUserData(code, placeholders);
        }

        public static (string Code, Dictionary<string, string> Placeholders) PreserveUserData(string in_code)
        {
            var placeholders = new Dictionary<string, string>();
            var index = 0;
            var guid = Guid.NewGuid();

            string Replace(Match in_match)
            {
                var key = $"__PLACEHOLDER_{guid}_{index++}__";

                placeholders[key] = in_match.Value;

                return key;
            }

            var pattern = @"""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])*'|--\[(=*)\[(.|\n)*?\]\1\]--|--[^\n]*|""""(?:\\.|[^""""])*""""|'(?:\\.|[^'])*'|\[=*\[(.|\n)*?\]=*\]";

            // Replace comments and strings with placeholders.
            in_code = Regex.Replace(in_code, pattern, new MatchEvaluator(Replace));

            return (in_code, placeholders);
        }

        public static string RestoreUserData(string in_code, Dictionary<string, string> in_placeholders)
        {
            foreach (var placeholder in in_placeholders)
                in_code = in_code.Replace(placeholder.Key, placeholder.Value);

            return in_code;
        }
    }
}
