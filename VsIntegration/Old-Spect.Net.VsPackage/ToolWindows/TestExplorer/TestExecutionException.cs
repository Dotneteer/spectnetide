using System;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This class represents exception raised by the test execution engine
    /// </summary>
    public class TestExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public TestExecutionException(string message) : base(message)
        {
        }
    }

}