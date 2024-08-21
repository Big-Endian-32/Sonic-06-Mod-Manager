using Marathon.Formats.Archive;
using Marathon.Formats.Package;
using Marathon.Helpers;
using Marathon.IO;
using SonicNextModManager.Helpers;
using SonicNextModManager.Metadata;

namespace SonicNextModManager.Patches
{
    public class ArchiveList : IDisposable
    {
        private string _archivePath;
        private U8Archive _archive;
        private U8ArchiveFile _file;
        private AssetPackage _pkg;

        public ArchiveList()
        {
            var gameDirectory = App.Settings.GetGameDirectory();

            if (string.IsNullOrEmpty(gameDirectory) || !Directory.Exists(gameDirectory))
                return;

            var core = App.GetCurrentPlatformString();

            _archivePath = Path.Combine(gameDirectory, core, @"archives\system.arc");
            _archive = new(_archivePath, ReadMode.IndexOnly);
            _file = (ArchiveHelper.GetFile(_archive.Root, $@"{core}\archive.pkg") as U8ArchiveFile)!;

            if (_file == null)
                return;

            _file.Decompress();

            _pkg = new();
            _pkg.Load(new MemoryStream(_file.Data));
        }

        public void Add(string in_relativePath)
        {
            var categoryName = "archive";

            if (in_relativePath.StartsWith("win32"))
                categoryName = "archive_win32";

            var category = _pkg.Categories.Where(x => x.Name == categoryName).FirstOrDefault()!;

            if (category == null)
                return;

            var fileNameNoExt = Path.GetFileNameWithoutExtension(in_relativePath).TrimStart(Database.CustomChar);

            in_relativePath = Path.Combine(Path.GetDirectoryName(in_relativePath)!, $"{fileNameNoExt}.arc");
            in_relativePath = in_relativePath.Replace('\\', '/');

            category.Files.Add(new(fileNameNoExt, in_relativePath[(in_relativePath.IndexOf('/') + 1)..]));
        }

        public void Add(List<string> in_paths)
        {
            foreach (var path in in_paths)
                Add(path);
        }

        public void Save()
        {
            Database.Backup(_archivePath);

            _file.Data = IOHelper.GetMarathonTypeBuffer(_pkg);
            _archive.Save();
        }

        public void Dispose()
        {
            Save();

            GC.SuppressFinalize(this);

            _pkg.Dispose();
            _file.Dispose();
            _archive.Dispose();
        }
    }
}
