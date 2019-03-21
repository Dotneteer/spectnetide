namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Represents a test exception that already has been handled
    /// </summary>
    public class HandledTestExecutionException : TestExecutionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
        /// </summary>
        public HandledTestExecutionException() : base("This test exception has already been handled.")
        {
        }
    }
}