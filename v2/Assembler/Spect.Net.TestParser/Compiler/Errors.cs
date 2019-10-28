using System.Collections.Generic;

namespace Spect.Net.TestParser.Compiler
{
    /// <summary>
    /// This class is responsible for managing error messages
    /// </summary>
    public static class Errors
    {
        // --- Error code constants
        public const string T0001 = "T0001";
        public const string T0002 = "T0002";
        public const string T0003 = "T0003";
        public const string T0004 = "T0004";
        public const string T0005 = "T0005";
        public const string T0006 = "T0006";
        public const string T0007 = "T0007";
        public const string T0008 = "T0008";
        public const string T0009 = "T0009";
        public const string T0010 = "T0010";
        public const string T0200 = "T0200";
        public const string T0201 = "T0201";

        // --- Error messages
        private static readonly IDictionary<string, string> s_Messages = new Dictionary<string, string>
        {
            {T0001, "Unexpected token: '{0}'"},
            {T0002, "A machine can have only these identifiers: 'SPECTRUM48', 'SPECTRUM128', 'SPECTRUMP3', 'NEXT'."},
            {T0003, "Z80 source file '{0}' cannot be found."},
            {T0004, "Z80 source file '{0}' compiled with {1} errors"},
            {T0005, "The {0} test option can be used only once"},
            {T0006, "Data member '{0}' is already declared in the test set"},
            {T0007, "Test block '{0}' is already declared in the test set"},
            {T0008, "Parameters '{0}' is already declared in the test block"},
            {T0009, "The test case #{0} contains {1} arguments, but the test block uses {2} parameters"},
            {T0010, "The test set does not has any port mock definition with name '{0}'"},
            {T0200, "Expression evaluation resulted an error: {0}" },
            {T0201, "This expression cannot be evaluated promptly, it may refer to an undefined symbol, a Z80 register, or a flag" },
        };

        /// <summary>
        /// Obtains the error message with the specified code.
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="parameters">Optional error parameters</param>
        /// <returns>Full error message</returns>
        public static string GetMessage(string errorCode, params object[] parameters)
        {
            return s_Messages.TryGetValue(errorCode, out var message)
                ? string.Format(message, parameters)
                : $"Undefined error message code {errorCode}";
        }
    }
}