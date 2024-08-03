using System.Collections.ObjectModel;

namespace SonicNextModManager
{
    public class Credits
    {
        public string? Category { get; set; }

        public string? GitHub { get; set; }

        public bool IsExpanded { get; set; }

        public class Contributor
        {
            public string? Name { get; set; }

            public string? Description { get; set; }

            public string? URL { get; set; }
        }

        public ObservableCollection<Contributor> Contributors { get; set; } = new();

        public static List<Credits> Parse()
            => JsonConvert.DeserializeObject<List<Credits>>(Properties.Resources.Credits);
    }
}
