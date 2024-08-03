namespace SonicNextModManager.UI.Dialogs
{
    /// <summary>
    /// Builds a Win32-compatible filter used for dialogs using a <see cref="Dictionary{TKey, TValue}"/> where TKey and TValue are strings.
    /// </summary>
    public class FilterBuilder
    {
        /// <summary>
        /// A collection of filters where the keys are the friendly names and the values are the wildcards.
        /// </summary>
        public Dictionary<string, string> Filters { get; set; } = new();

        /// <summary>
        /// The resulting filter compatible with Win32 dialogs.
        /// </summary>
        public string? Result { get; private set; }

        public FilterBuilder()
            => Build();

        public FilterBuilder(Dictionary<string, string> filters)
        {
            Filters = filters;

            Build();
        }

        private void Build()
        {
            StringBuilder filterBuilder = new();
            {
                // Create filter using dictionary values.
                foreach (var entry in Filters)
                    filterBuilder.Append($"{entry.Key} ({entry.Value})|{entry.Value.Replace(" ", "")}|");

                // Remove last pipe to make it a valid filter.
                filterBuilder.Remove(filterBuilder.Length - 1, 1);
            }

            Result = filterBuilder.ToString();
        }
    }
}
