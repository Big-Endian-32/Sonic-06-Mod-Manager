namespace SonicNextModManager.Patches
{
    public class AsmHook(uint in_address, byte[] in_code)
    {
        public uint Address { get; set; } = in_address;

        public byte[] Code { get; set; } = in_code;
    }
}
