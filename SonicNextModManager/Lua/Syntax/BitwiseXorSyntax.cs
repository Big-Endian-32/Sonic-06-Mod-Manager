using SonicNextModManager.Lua.Syntax.Interfaces;

namespace SonicNextModManager.Lua.Syntax
{
    public class BitwiseXorSyntax : BitwiseSyntax, ICustomSyntax
    {
        public string Install(string in_code)
        {
            return Install(in_code, "^");
        }
    }
}
