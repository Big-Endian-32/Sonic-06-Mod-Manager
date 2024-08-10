using SonicNextModManager.Lua.Syntax.Interfaces;

namespace SonicNextModManager.Lua.Syntax
{
    public class BitwiseAndSyntax : BitwiseSyntax, ICustomSyntax
    {
        public string Install(string in_code)
        {
            return Install(in_code, "&");
        }
    }
}
