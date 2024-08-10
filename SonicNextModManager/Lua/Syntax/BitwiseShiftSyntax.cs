using SonicNextModManager.Lua.Syntax.Interfaces;

namespace SonicNextModManager.Lua.Syntax
{
    public class BitwiseShiftSyntax : BitwiseSyntax, ICustomSyntax
    {
        public string Install(string in_code)
        {
            in_code = Install(in_code, "<<");
            in_code = Install(in_code, ">>");

            return in_code;
        }
    }
}
