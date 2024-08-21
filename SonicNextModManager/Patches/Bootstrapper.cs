using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Patches
{
    public class Bootstrapper
    {
        public static List<AsmHook> Hooks { get; } = [];

        public static string GetHookDefinitions()
        {
            var sb = new StringBuilder("[Hooks]\n");

            foreach (var hook in Hooks)
            {
                if (hook.Address < 0x82000000)
                    hook.Address = MemoryHelper.ToVirtual(hook.Address);

                sb.AppendLine($"0x{hook.Address:X8}=\"{MemoryHelper.ByteArrayToHexString(hook.Code, false)}\"");
            }

            return sb.ToString();
        }

        public static void Install()
        {
            // TODO: https://github.com/NotNite/SPRXPatcher
            if (App.GetCurrentPlatform() != EPlatform.Xbox)
                throw new NotImplementedException();

            var gameExecutable = App.Settings.Path_GameExecutable;

            if (string.IsNullOrEmpty(gameExecutable))
                return;

            // Shellcode by Reimous-TH.
            using (var fs = File.OpenWrite(gameExecutable))
            {
                var ms = new MemoryService(fs);

                // Redirect DbgPrint to XexLoadImage.
                ms.WriteBytes(MemoryHelper.ToPhysical(0x82AEA31E), [0x01, 0x99]);
                ms.WriteBytes(MemoryHelper.ToPhysical(0x82AEA322), [0x01, 0x99]);

                // Branch to bootstrapper code from entrypoint.
                ms.WriteBytes(MemoryHelper.ToPhysical(0x82537AF0), [0x4B, 0xC7, 0xDC, 0xC1]);

                // Write bootstrapper code to unused subroutine.
                ms.WriteBytes
                (
                    MemoryHelper.ToPhysical(0x821B57B8),

                    AssemblerService.Assemble
                    (
                        @"
                            stwu %r1, -0x100(%r1)
                            lis  %r11, 0x821B
                            addi %r3, %r11, 0x57F0 ; szXexName
                            li   %r4, 9            ; dwModuleTypeFlags = XEX_MODULE_TYPE_TITLE_DLL
                            li   %r5, 0            ; dwMinimumVersion
                            addi %r6, %r1, 0x50
                            lis  %r0, 0x82AE
                            ori  %r0, %r0, 0xA31C  ; 0x82AEA31C (DbgPrint, now XexLoadImage)
                            mtlr %r0
                            blrl                   ; Call XexLoadImage
                            addi %r1, %r1, 0x100
                            lwz  %r12, -8(%r1)
                            mtlr %r12
                            blr
                        "
                    )
                );

                // Write loader module path.
                ms.WriteString(MemoryHelper.ToPhysical(0x821B57F0), "game:\\SonicNextModLoader.xex\0");
            }

            // TODO: write SonicNextModLoader.xex.
            File.WriteAllText(Path.Combine(App.Settings.GetGameDirectory()!, "SonicNextModLoader.ini"), GetHookDefinitions());
        }

        public static void Uninstall()
        {
            Hooks.Clear();

            var modLoaderPath = Path.Combine(App.Settings.GetGameDirectory()!, "SonicNextModLoader");

            if (!Directory.Exists(modLoaderPath))
                return;

            var modLoaderIniPath = modLoaderPath + ".ini";
            var modLoaderDllPath = modLoaderPath + ".xex"; // TODO: change to SPRX for PS3.

            if (File.Exists(modLoaderIniPath))
                File.Delete(modLoaderIniPath);

            if (File.Exists(modLoaderDllPath))
                File.Delete(modLoaderDllPath);
        }
    }
}
