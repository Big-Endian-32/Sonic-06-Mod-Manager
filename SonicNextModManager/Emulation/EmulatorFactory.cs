using SonicNextModManager.Metadata;
using System.Diagnostics;

namespace SonicNextModManager.Emulation
{
    public class EmulatorFactory
    {
        public static ProcessStartInfo GetStartInfo()
        {
            var result = new ProcessStartInfo()
            {
                FileName = App.Settings.Path_EmulatorExecutable,
                WorkingDirectory = Path.GetDirectoryName(App.Settings.Path_EmulatorExecutable),
            };

            if (!string.IsNullOrEmpty(App.Settings.Emulator_Arguments))
            {
                foreach (var arg in App.Settings.Emulator_Arguments.Split(' '))
                    result.ArgumentList.Add(arg);
            }

            switch (App.GetCurrentPlatform())
            {
                case EPlatform.Xbox:
                {
                    result.ArgumentList.Add($"--gpu={App.Settings.Emulator_Xenia_Backend.ToString().ToLower()}");
                    result.ArgumentList.Add($"--draw_resolution_scale_x={App.Settings.Emulator_Xenia_Width + 1}");
                    result.ArgumentList.Add($"--draw_resolution_scale_y={App.Settings.Emulator_Xenia_Height + 1}");
                    result.ArgumentList.Add($"--vsync=true");
                    result.ArgumentList.Add($"--kernel_display_gamma_type={(App.Settings.Emulator_Xenia_IsGammaCorrection ? 2 : 0)}");

                    if (App.Settings.Emulator_Xenia_IsFullscreen)
                        result.ArgumentList.Add("--fullscreen");

                    result.ArgumentList.Add($"--user_language={(int)App.Settings.Emulator_Xenia_Language + 1}");

                    break;
                }
            }

            result.ArgumentList.Add(App.Settings.Path_GameExecutable);

            return result;
        }
    }
}
