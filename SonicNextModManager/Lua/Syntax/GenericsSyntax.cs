using SonicNextModManager.Lua.Syntax.Interfaces;
using System.Text.RegularExpressions;

namespace SonicNextModManager.Lua.Syntax
{
    public class GenericsSyntax : SanitisedSyntaxBase, ICustomSyntax
    {
        public string Install(string in_code)
        {
            in_code = InstallSanitised(in_code, InstallGeneric);

            return in_code;
        }

        public string InstallGeneric(string in_code)
        {
            var pattern = $@"(\w+)<([^>]+)>\s*\(";
            var matches = Regex.Matches(in_code, pattern);

            if (matches.Count <= 0)
                return in_code;

            foreach (Match match in matches)
            {
                var functionName = match.Groups[1].Value;
                var genericTypes = match.Groups[2].Value;

                var types = genericTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < types.Length; i++)
                    types[i] = $"\"{types[i]}\"";

                genericTypes = string.Join(", ", types);

                in_code = in_code.Replace(match.Value, $"{functionName}({genericTypes}, ");
            }

            return in_code;
        }
    }
}
