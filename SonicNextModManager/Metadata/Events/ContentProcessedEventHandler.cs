namespace SonicNextModManager.Metadata.Events
{
    public class ContentProcessedEventArgs(string in_name, int in_index, int in_total) : EventArgs
    {
        /// <summary>
        /// The name of this content.
        /// </summary>
        public string Name { get; } = in_name;

        /// <summary>
        /// The index of this content in the list.
        /// </summary>
        public int Index { get; } = in_index;

        /// <summary>
        /// The total number of content items in the list.
        /// </summary>
        public int Total { get; } = in_total;
    }

    public delegate void ContentProcessedEventHandler(object in_sender, ContentProcessedEventArgs in_args);
}
