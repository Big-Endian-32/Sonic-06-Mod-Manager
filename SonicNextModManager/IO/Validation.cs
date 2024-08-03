using SonicNextModManager.Helpers;

namespace SonicNextModManager.IO
{
    public class Validation
    {
        private static List<string>? _gameData = new();

        /// <summary>
        /// Enumerates a list of files pertaining to the game's data.
        /// </summary>
        public static IEnumerable<string>? EnumerateGameData()
        {
            // Return null if there's no game executable set.
            if (!File.Exists(App.Settings.Path_GameExecutable))
                return null;

            // Returns the list if it's populated.
            if (_gameData.Count != 0)
                return _gameData;

            // Parses the JSON and returns the result.
            if (File.Exists(App.Configurations["Data"]))
                return _gameData = JsonConvert.DeserializeObject<List<string>?>(File.ReadAllText(App.Configurations["Data"]));

            // Enumerates the list of game files.
            foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(App.Settings.Path_GameExecutable), "*.*", SearchOption.AllDirectories))
                _gameData.Add(StringHelper.OmitSourceDirectory(file, Path.GetDirectoryName(App.Settings.Path_GameExecutable)));

            // Saves the list to JSON.
            File.WriteAllText(App.Configurations["Data"], JsonConvert.SerializeObject(_gameData));

            // Return new game data list.
            return _gameData;
        }
    }
}
