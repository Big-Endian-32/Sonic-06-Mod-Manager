using HandyControl.Controls;
using SonicNextModManager.Helpers;
using SonicNextModManager.Networking;
using SonicNextModManager.Services;
using SonicNextModManager.UI.Components;
using SonicNextModManager.UI.Dialogs;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SonicNextModManager.UI
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : ImmersiveWindow
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => SonicNextModManager.Language.UpdateCultureResources();

        private void Game_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set Next button enabled state.
            DialogButtons.GetButton(LocaleService.Localise("Common_Next")).IsEnabled = Game_Path.Text.Length != 0;

            // Set global game executable path.
            App.Settings.Path_GameExecutable = Game_Path.Text;
        }

        private void Game_Browse_Click(object sender, RoutedEventArgs e)
        {
            string executable = FileQueries.QueryGameExecutable();

            if (!string.IsNullOrEmpty(executable))
                Game_Path.Text = executable;
        }

        private void Emulator_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set Next button enabled state.
            DialogButtons.GetButton(LocaleService.Localise("Common_Next")).IsEnabled = Emulator_Path.Text.Length != 0;

            // Set global emulator executable path.
            App.Settings.Path_EmulatorExecutable = Emulator_Path.Text;
        }

        private void Emulator_Browse_Click(object sender, RoutedEventArgs e)
        {
            string executable = FileQueries.QueryEmulatorExecutable();

            if (!string.IsNullOrEmpty(executable))
                Emulator_Path.Text = executable;
        }

        /// <summary>
        /// Displays a full preview upon double-clicking an image in a carousel.
        /// </summary>
        private void Image_Carousel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                new ImageBrowser(new Uri(((BitmapFrame)((Image)sender).Source).Decoder.ToString())).Show();
        }

        private void StateMachine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DialogButtons == null)
                return;

            DialogButtons.Buttons.Clear();

            if (MajorStepPages.SelectedItem == MajorStep_Welcome)
            {
                DialogButtons.AddButton(LocaleService.Localise("Common_Next"), () => SetCurrentStep(1));
            }
            else if (MajorStepPages.SelectedItem == MajorStep_Game)
            {
                DialogButtons.AddButton(LocaleService.Localise("Common_Next"), () => SetCurrentStep(1)).IsEnabled = Game_Path.Text.Length != 0;
                DialogButtons.AddButton(LocaleService.Localise("Common_Back"), () => SetCurrentStep(-1));
            }
            else if (MajorStepPages.SelectedItem == MajorStep_Emulator)
            {
                if (EmulatorStepPages.SelectedItem == EmulatorStep_Prompt)
                {
                    DialogButtons.AddButton(LocaleService.Localise("Common_No"), () => SetCurrentStep(1));

                    DialogButtons.AddButton(LocaleService.Localise("Common_Yes"), () =>
                    {
                        Task<string> supportedEmulators;

                        try
                        {
                            supportedEmulators = Client.Get().GetStringAsync
                            (
                                StringHelper.URLCombine
                                (
                                    Properties.Resources.GitHub_Raw,
                                    Properties.Resources.GitHub_User,
                                    Properties.Resources.GitHub_Repository,
                                    "next/SonicNextModManager/Resources/Networking/Emulators.json"
                                )
                            );

                            // Attempt to connect to GitHub.
                            supportedEmulators.Wait();
                        }
                        catch
                        {
                            EmulatorStepPages.SelectedItem = EmulatorStep_NetworkError;

                            return;
                        }

                        // Parse flags from JSON.
                        var emulatorFlags = JsonConvert.DeserializeObject<Dictionary<string, bool>>(supportedEmulators.Result);

                        switch (Path.GetExtension(Game_Path.Text))
                        {
                            case ".xex":
                            {
                                if (emulatorFlags["IsXeniaBugFree"])
                                    break;

                                EmulatorStepPages.SelectedItem = EmulatorStep_Xenia;
                                return;
                            }

                            case ".bin":
                            {
                                if (emulatorFlags["IsRPCS3BugFree"])
                                    break;

                                EmulatorStepPages.SelectedItem = EmulatorStep_RPCS3;
                                return;
                            }
                        }

                        // Jump another major page if all cases pass.
                        SetCurrentStep(1);
                    });
                }
                else if (EmulatorStepPages.SelectedItem == EmulatorStep_Xenia)
                {
                    DialogButtons.AddButton(LocaleService.Localise("Common_Next"), () => EmulatorStepPages.SelectedItem = EmulatorStep_Setup);
                    DialogButtons.AddButton(LocaleService.Localise("Common_Back"), () => ToEmulatorStepRoot());
                }
                else if (EmulatorStepPages.SelectedItem == EmulatorStep_RPCS3)
                {
                    DialogButtons.AddButton(LocaleService.Localise("Common_Back"), () => ToEmulatorStepRoot());
                }
                else if (EmulatorStepPages.SelectedItem == EmulatorStep_NetworkError)
                {
                    DialogButtons.AddButton(LocaleService.Localise("Common_Next"), () => SetCurrentStep(1));
                    DialogButtons.AddButton(LocaleService.Localise("Common_Back"), () => ToEmulatorStepRoot());
                }
                else if (EmulatorStepPages.SelectedItem == EmulatorStep_Setup)
                {
                    DialogButtons.AddButton(LocaleService.Localise("Common_Next"), () => SetCurrentStep(1)).IsEnabled = Emulator_Path.Text.Length != 0;
                    DialogButtons.AddButton(LocaleService.Localise("Common_Back"), () => SetCurrentStep(-1));
                }

                void ToEmulatorStepRoot()
                {
                    EmulatorStepPages.SelectedIndex = 0;
                    SetCurrentStep(-1);
                }
            }
            else if (MajorStepPages.SelectedItem == MajorStep_Finish)
            {
                DialogButtons.AddButton(LocaleService.Localise("Setup_Finish_OK"), () =>
                {
                    // Set completion flag.
                    App.Settings.Setup_Complete = true;

                    // Load mod manager window.
                    new Manager().Show();

                    // Close current window.
                    Close();
                });
            }

            void SetCurrentStep(int index)
            {
                Steps.StepIndex += index;
                MajorStepPages.SelectedIndex += index;
            }
        }
    }
}
