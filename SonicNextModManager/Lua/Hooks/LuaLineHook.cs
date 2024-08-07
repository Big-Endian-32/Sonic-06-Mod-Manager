using SonicNextModManager.Helpers;
using System.Text.RegularExpressions;

namespace SonicNextModManager.Lua.Hooks
{
    public class LuaLineHook : LuaHook
    {
        public string Code { get; }

        public string Pattern { get; }

        public bool IsRegex { get; } = false;

        public ERegexOptions RegexOptions { get; }

        public LuaLineHook(string in_code, string in_pattern, EHookBehaviour in_behaviour, bool in_isRegex = false, ERegexOptions in_regexOptions = default) : base(in_behaviour)
        {
            Code = in_code;
            Pattern = in_pattern;
            IsRegex = in_isRegex;
            RegexOptions = in_regexOptions;
            Hash = ToString().GetHashCode();
        }

        public override string WriteHook(string in_code)
        {
            var result = in_code;

            var regexOptions = ERegexOptions.Compiled | RegexOptions;

            var isLineFound = IsRegex
                ? Regex.Match(result, Pattern, (RegexOptions)regexOptions).Success
                : result.Contains(Pattern);

            if (!isLineFound)
                return result;

            var insertions = new List<(int Index, string Text)>();

            void AddInsertion(int in_index, int in_length, string in_indentation)
            {
                switch (Behaviour)
                {
                    case EHookBehaviour.Before:
                        insertions.Add((in_index, $"{Code}\n{in_indentation}"));
                        break;

                    case EHookBehaviour.Replace:
                    {
                        result = IsRegex
                            ? new Regex(Pattern, (RegexOptions)regexOptions).Replace(result, Code)
                            : result.Replace(Pattern, Code);

                        break;
                    }

                    case EHookBehaviour.After:
                        insertions.Add((in_index + in_length, $"\n{in_indentation}{Code}"));
                        break;
                }
            }

            string GetIndentation(int in_index)
            {
                var indentIndex = in_index;
                var indentCount = 0;

                while (result[indentIndex] == ' ')
                {
                    if (indentIndex <= 0)
                        break;

                    indentIndex--;
                    indentCount++;
                }

                return new string(' ', indentCount);
            }

            if (IsRegex)
            {
                foreach (Match match in Regex.Matches(result, Pattern, (RegexOptions)regexOptions))
                    AddInsertion(match.Index, match.Length, GetIndentation(match.Index - 1));
            }
            else
            {
                foreach (int index in StringHelper.GetSubstringIndices(result, Pattern))
                    AddInsertion(index, Pattern.Length, GetIndentation(index - 1));
            }

            foreach (var insertion in insertions.OrderByDescending(i => i.Index))
                result = result.Insert(insertion.Index, insertion.Text);

            return result;
        }

        public override string ToString()
        {
            return $"[Lua] Line Hook - [\"{Pattern}\"] (Behaviour: {Behaviour}, Regex: {IsRegex});\n{Code}";
        }
    }
}
