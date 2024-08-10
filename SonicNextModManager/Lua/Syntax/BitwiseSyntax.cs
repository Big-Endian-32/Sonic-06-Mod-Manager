using System.Text.RegularExpressions;

namespace SonicNextModManager.Lua.Syntax
{
    public class BitwiseSyntax
    {
        public string Install(string in_code, string in_operator)
        {
            in_code = InstallBitwise(in_code, in_operator);
            in_code = InstallBitwiseAssignment(in_code, in_operator);

            return in_code;
        }

        public string InstallBitwise(string in_code, string in_operator)
        {
            var pattern = $@"\b(?:[\w\[\]\.]+)\b\s*\{in_operator}\s*(?:\w+\.\w+|\w+)";
            var matches = Regex.Matches(in_code, pattern);

            if (matches.Count <= 0)
                return in_code;

            foreach (Match match in matches)
            {
                var split = match.Value.Split(in_operator, StringSplitOptions.TrimEntries);
                var exprs = split.Aggregate((c, n) => $"bit32.{GetBit32Method(in_operator)}({c}, {n})");

                in_code = in_code.Replace(match.Value, exprs);
            }

            return in_code;
        }

        public string InstallBitwiseAssignment(string in_code, string in_operator)
        {
            var pattern = $@"([^\s|]+)\s*\{in_operator}\=\s*([^\s|]+.*?)(?=\s*\n|$)";
            var replace = $@"$1 = bit32.{GetBit32Method(in_operator)}($1, $2)";

            return Regex.Replace(in_code, pattern, replace);
        }

        public static string GetBit32Method(string in_operator)
        {
            return in_operator switch
            {
                "&"  => "band",
                "|"  => "bor",
                "^"  => "bxor",
                "<<" => "lshift",
                ">>" => "rshift",
                _    => throw new NotImplementedException(),
            };
        }
    }
}
