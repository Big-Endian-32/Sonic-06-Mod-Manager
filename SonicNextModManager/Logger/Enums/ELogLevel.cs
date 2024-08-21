namespace SonicNextModManager.Logger
{
    internal enum ELogLevel
    {
        /// <summary>
        /// None - used for user output.
        /// <para>Colour: White</para>
        /// </summary>
        None,

        /// <summary>
        /// Utility - usually reserved for debugging.
        /// <para>Colour: Green</para>
        /// </summary>
        Utility,

        /// <summary>
        /// Warnings - reserved for minor issues that aren't critical.
        /// <para>Colour: Yellow</para>
        /// </summary>
        Warning,

        /// <summary>
        /// Errors - reserved for fatal issues.
        /// <para>Colour: Red</para>
        /// </summary>
        Error
    }
}
