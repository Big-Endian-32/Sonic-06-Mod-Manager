using Gee.External.Capstone.PowerPc;
using Keystone;
using SonicNextModManager.Logger;

namespace SonicNextModManager.Services
{
    public class AssemblerService
    {
        /// <summary>
        /// Assembles PowerPC assembly code.
        /// </summary>
        /// <param name="in_code">The code to assemble.</param>
        /// <param name="in_addr">The address of the first instruction to encode (optional).</param>
        /// <returns>A byte array containing PowerPC bytecode.</returns>
        public static byte[] Assemble(string in_code, uint in_addr = 0)
        {
            // Remove comments from code blocks (ignore for single lines).
            if (in_code.Contains('\n') || in_code.Contains('\r'))
                in_code = RemoveComments(in_code);

#if !DEBUG
            try
#endif
            {
                using (var keystone = new Engine(Architecture.PPC, Mode.PPC64 | Mode.BIG_ENDIAN) { ThrowOnError = true })
                    return keystone.Assemble(in_code, in_addr).Buffer;
            }
#if !DEBUG
            catch (KeystoneException ex)
            {
                LoggerService.Error($"Failed to assemble instructions.\n{ex}");
                return [];
            }
#endif
        }

        /// <summary>
        /// Disassembles PowerPC bytecode.
        /// </summary>
        /// <param name="in_code">The bytecode to disassemble.</param>
        public static PowerPcInstruction[] Disassemble(byte[] in_code)
        {
            using (var disassembler = new CapstonePowerPcDisassembler(PowerPcDisassembleMode.BigEndian | PowerPcDisassembleMode.Bit64))
                return disassembler.Disassemble(in_code);
        }

        private static string RemoveComments(string in_code)
        {
            var lines = in_code.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                int commentIndex = lines[i].IndexOf(';');

                if (commentIndex >= 0)
                    lines[i] = lines[i][..commentIndex].TrimEnd();
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
