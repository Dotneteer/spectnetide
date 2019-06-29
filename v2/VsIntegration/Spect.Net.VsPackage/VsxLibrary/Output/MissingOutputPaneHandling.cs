namespace Spect.Net.VsPackage.VsxLibrary.Output
{
    /// <summary>
    /// This class defines how output window pane exceptions should be handled.
    /// </summary>
    public enum MissingOutputPaneHandling
    {
        /// <summary>
        /// Exceptions should be caught and never raised.
        /// </summary>
        Silent,

        /// <summary>
        /// Exceptions should be raised.
        /// </summary>
        ThrowException,

        /// <summary>
        /// In case of exceptions, output should be redirected to the General pane.
        /// </summary>
        RedirectToGeneral
    }
}