namespace SonicNextModManager.Lua.Hooks
{
    public abstract class LuaHook
    {
        public EHookBehaviour Behaviour { get; }

        public int Hash { get; set; }

        public LuaHook(EHookBehaviour in_behaviour)
        {
            Behaviour = in_behaviour;
        }

        public abstract string WriteHook(string in_code);

        public override bool Equals(object? in_obj)
        {
            if (in_obj == null || GetType() != in_obj.GetType())
                return false;

            return Hash == ((LuaHook)in_obj).Hash;
        }

        public override int GetHashCode()
        {
            return Hash.ToString("X").GetHashCode();
        }
    }
}
