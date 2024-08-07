using System.ComponentModel;

namespace SonicNextModManager.Metadata
{
    public class MetadataBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The title of this content.
        /// </summary>
        public string? Title { get; set; } = LocaleService.Localise("Common_NotAvailable");

        /// <summary>
        /// The author of this content.
        /// </summary>
        public string? Author { get; set; } = LocaleService.Localise("Common_NotAvailable");

        /// <summary>
        /// The platform this content is targeting.
        /// </summary>
        public EPlatform Platform { get; set; } = EPlatform.Any;

        /// <summary>
        /// The date this content was created on.
        /// </summary>
        public string? Date { get; set; } = "14/11/2006";

        /// <summary>
        /// Initialiser for <see cref="Description"/>.
        /// </summary>
        private string? _Description = LocaleService.Localise("Patches_NoInfo");

        /// <summary>
        /// The description of this content.
        /// </summary>
        public string? Description
        {
            get => _Description?.Replace("\\n", Environment.NewLine);

            set => _Description = value;
        }

        /// <summary>
        /// The path to the thumbnail used by this content.
        /// </summary>
        [JsonIgnore]
        public string? Thumbnail { get; set; }

        /// <summary>
        /// Determines if this content is enabled.
        /// </summary>
        [JsonIgnore]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Determines if this content's information is being displayed.
        /// </summary>
        [JsonIgnore]
        public bool IsInfoDisplay { get; set; }

        /// <summary>
        /// The state of this content's installation process.
        /// </summary>
        [JsonIgnore]
        public EInstallState State { get; set; }

        /// <summary>
        /// The path to this content.
        /// </summary>
        [JsonIgnore]
        public string? Location { get; set; }
    }
}
