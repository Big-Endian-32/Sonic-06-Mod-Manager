using SonicNextModManager.Extensions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SonicNextModManager.Lua.Hooks
{
    public class LuaFunctionHook : LuaHook
    {
        public string Code { get; }

        public string FunctionName { get; }

        public LuaFunctionHook(string in_code, string in_functionName, EHookBehaviour in_behaviour) : base(in_behaviour)
        {
            Code = in_code.Indent(1);
            FunctionName = in_functionName;
            Hash = ToString().GetHashCode();
        }

        public override string WriteHook(string in_code)
        {
            var result = new StringBuilder();
            var funcPattern = $@"(^function\s+{FunctionName}\s*\(.*?\))";
            var isHooked = false;
            var isInsideFunction = false;
            var scopeDepth = 0;

            var nestedKeywords = new List<string>()
            {
                "function",
                "if",
                "elseif",
                "for",
                "while",
                "repeat"
            };

            foreach (string line in in_code.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                if (isHooked)
                {
                    result.AppendLine(line);
                    continue;
                }

                if (line.Contains($"function {FunctionName}"))
                {
                    /* Handle single line function closures.
                       (e.g. "function myFunction() doSomething() end") */
                    if (line.TrimEnd().EndsWith("end"))
                    {
                        Match inlineFuncSig = Regex.Match(line, funcPattern, RegexOptions.Compiled);

                        if (!inlineFuncSig.Success)
                            continue;

                        var funcSig = inlineFuncSig.Groups[0].Value;
                        var funcSigIndex = inlineFuncSig.Index + inlineFuncSig.Length;

                        var originalCode = line.Substring(funcSigIndex, line.LastIndexOf("end") - funcSigIndex).Indent(1);

                        switch (Behaviour)
                        {
                            case EHookBehaviour.Before:
                                result.Append($"{funcSig}\n{Code}\n{originalCode}\nend");
                                break;

                            case EHookBehaviour.Replace:
                                result.Append($"{funcSig}\n{Code}\nend");
                                break;

                            case EHookBehaviour.After:
                                result.Append($"{funcSig}\n{originalCode}\n{Code}\nend");
                                break;
                        }

                        isInsideFunction = false;
                        scopeDepth = 0;

                        continue;
                    }

                    // Write function signature.
                    result.AppendLine(line);

                    if (Behaviour == EHookBehaviour.Before)
                        result.AppendLine(Code);

                    isInsideFunction = true;
                    scopeDepth++;

                    continue;
                }
                else if (isInsideFunction)
                {
                    if (line.Trim() == "end")
                    {
                        scopeDepth--;

                        if (scopeDepth == 0)
                        {
                            if (Behaviour != EHookBehaviour.Before)
                                result.AppendLine(Code);

                            result.AppendLine("end");

                            isHooked = true;
                            isInsideFunction = false;

                            continue;
                        }
                    }
                    else if (nestedKeywords.Any(x => line.Trim().StartsWith(x)))
                    {
                        scopeDepth++;
                    }

                    if (Behaviour != EHookBehaviour.Replace)
                        result.AppendLine(line);

                    continue;
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }

        public override string ToString()
        {
            return $"[Lua] Function Hook - [{FunctionName}] (Behaviour: {Behaviour});\n{Code}";
        }
    }
}
