using Marathon.Exceptions;
using Marathon.Formats.Archive;
using Marathon.Formats.Script.Lua;
using SonicNextModManager.Extensions;
using SonicNextModManager.Helpers;
using SonicNextModManager.Lua.Attributes;
using SonicNextModManager.Lua.Hooks;
using System.Diagnostics;

namespace SonicNextModManager.Lua.Wrappers.Script
{
    [LuaUserData]
    public class LuaBinaryWrapper : MarathonWrapper
    {
        private LuaBinary _luaBinary;
        private string[] _luaCode;

        private List<LuaHook> _hooks = [];

        public LuaBinaryWrapper() { }

        public LuaBinaryWrapper(U8ArchiveFile in_file) : base(in_file)
        {
            _luaBinary = IOHelper.LoadMarathonTypeFromBuffer<LuaBinary>(File.Data);

            try
            {
                var code = _luaBinary.Decompile();

                _luaCode = code.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            }
            catch (InvalidSignatureException)
            {
#if DEBUG
                Debug.WriteLine($"Attempted to decompile an already decompiled Lua script: {in_file.Name}");
#endif
            }
        }

        public string this[int in_index]
        {
            get => _luaCode[in_index];
            set => _luaCode[in_index] = value;
        }

        public DynValue this[string in_name]
        {
            get => GetConstant(in_name);
            set => SetConstant(in_name, value);
        }

        public string[] GetLines()
        {
            return _luaCode;
        }

        private int GetDeclarationLine(string in_name)
        {
            for (int i = 0; i < _luaCode.Length; i++)
            {
                var line = _luaCode[i];

                if (line.TrimStart().StartsWith(in_name) && line.Contains('='))
                    return i;
            }

            return -1;
        }

        public DynValue GetConstant(string in_name)
        {
            var decl = GetDeclarationLine(in_name);

            if (decl == -1)
                return DynValue.Nil;

            var line = GetLines()[decl];

            return MoonSharpHelper.TransformStringToDynValue(line[(line.IndexOf('=') + 1)..].Trim());
        }

        public void SetConstant(string in_name, DynValue in_value)
        {
            var decl = GetDeclarationLine(in_name);

            if (decl == -1)
                return;

            _luaCode[decl] = $"{in_name} = {in_value}";
        }

        public void CreateLineHook(string in_code, string in_pattern, EHookBehaviour in_behaviour,
            bool in_isRegex = false, ERegexOptions in_regexOptions = default)
        {
            var hook = new LuaLineHook(in_code, in_pattern, in_behaviour, in_isRegex, in_regexOptions);

            if (_hooks.Contains(hook))
                return;

            _hooks.Add(hook);
        }

        public void CreateFunctionHook(string in_code, string in_functionName, EHookBehaviour in_behaviour)
        {
            var hook = new LuaFunctionHook(in_code, in_functionName, in_behaviour);

            if (_hooks.Contains(hook))
                return;

            _hooks.Add(hook);
        }

        public void Append(string in_code)
        {
            _luaCode = [.. _luaCode, .. in_code.GetLines()];
        }

        public void Close()
        {
            var code = string.Join('\n', _luaCode);

            foreach (var hook in _hooks)
                code = hook.WriteHook(code);

            File.Data = Encoding.UTF8.GetBytes(code);
        }
    }
}
