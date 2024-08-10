using SonicNextModManager.Lua.Syntax.Interfaces;

namespace SonicNextModManager.Lua.Syntax
{
    public class BitwiseOrSyntax : BitwiseSyntax, ICustomSyntax
    {
        public string Install(string in_code)
        {
            return Install(in_code, "|");
        }
    }
}
