using System.Threading.Tasks;
using SonicNextModManager.Helpers;
using SonicNextModManager.Networking.API.GitHub;

namespace SonicNextModManager
{
    public class GitHub
    {
        private const string API = "https://api.github.com";
        private const string Repositories = "repos";

        /// <summary>
        /// Gets the latest release from the input repository.
        /// </summary>
        /// <param name="username">GitHub username.</param>
        /// <param name="repository">GitHub repository name.</param>
        public static Task<ReleaseInfo> GetLatestRelease(string username, string repository)
        {
            return JsonExtensions.DeserializeWebObjectAsync<ReleaseInfo>
            (
                StringHelper.URLCombine(API, Repositories, username, repository, "releases", "latest")
            );
        }

        /// <summary>
        /// Gets the latest release from the default repository.
        /// </summary>
        public static Task<ReleaseInfo> GetLatestRelease()
            => GetLatestRelease(Properties.Resources.GitHub_User, Properties.Resources.GitHub_Repository);

        /// <summary>
        /// Creates an issue on the GitHub issue tracker.
        /// </summary>
        /// <param name="username">GitHub username.</param>
        /// <param name="repository">GitHub repository name.</param>
        /// <param name="title">Title used for the issue.</param>
        /// <param name="body">Text automatically added to the issue.</param>
        /// <param name="labels">Labels automatically added to the issue.</param>
        public static void CreateNewIssue(string username, string repository, string title = "", string body = "", List<string> labels = null)
        {
            ProcessExtensions.StartWithDefaultProgram
            (
                StringHelper.URLCombine
                (
                    $"https://github.com/{username}/{repository}/issues/new",

                    Uri.EscapeUriString
                    (
                        $"?title={title}" +
                        $"&body={body}" +
                        $"&labels={(labels == null ? string.Empty : string.Join(", ", labels))}"
                    )
                )
            );
        }

        /// <summary>
        /// Creates an issue on the default GitHub issue tracker.
        /// </summary>
        /// <param name="title">Title used for the issue.</param>
        /// <param name="body">Text automatically added to the issue.</param>
        /// <param name="labels">Labels automatically added to the issue.</param>
        public static void CreateNewIssue(string title = "", string body = "", List<string> labels = null)
            => CreateNewIssue(Properties.Resources.GitHub_User, Properties.Resources.GitHub_Repository, title, body, labels);
    }
}
