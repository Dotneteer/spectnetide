using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class is responsible for managing error messages
    /// </summary>
    public static class Errors
    {
        // --- Error code constants
        public const string Z0001 = "Z0001";
        public const string Z0002 = "Z0002";
        public const string Z0004 = "Z0004";
        public const string Z0005 = "Z0005";
        public const string Z0006 = "Z0006";
        public const string Z0007 = "Z0007";
        public const string Z0008 = "Z0008";
        public const string Z0009 = "Z0009";
        public const string Z0010 = "Z0010";
        public const string Z0011 = "Z0011";
        public const string Z0012 = "Z0012";
        public const string Z0013 = "Z0013";
        public const string Z0014 = "Z0014";
        public const string Z0015 = "Z0015";
        public const string Z0016 = "Z0016";
        public const string Z0017 = "Z0017";
        public const string Z0018 = "Z0018";
        public const string Z0019 = "Z0019";
        public const string Z0020 = "Z0020";
        public const string Z0021 = "Z0021";
        public const string Z0022 = "Z0022";
        public const string Z0023 = "Z0023";
        public const string Z0040 = "Z0040";
        public const string Z0060 = "Z0060";
        public const string Z0061 = "Z0061";
        public const string Z0062 = "Z0062";
        public const string Z0080 = "Z0080";
        public const string Z0081 = "Z0081";
        public const string Z0082 = "Z0082";
        public const string Z0083 = "Z0083";
        public const string Z0084 = "Z0084";
        public const string Z0085 = "Z0085";
        public const string Z0086 = "Z0086";
        public const string Z0087 = "Z0087";
        public const string Z0088 = "Z0088";
        public const string Z0089 = "Z0089";
        public const string Z0090 = "Z0090";

        public const string Z0100 = "Z0100";
        public const string Z0101 = "Z0101";

        public const string Z0200 = "Z0200";
        public const string Z0201 = "Z0201";

        public const string Z0300 = "Z0300";
        public const string Z0301 = "Z0301";
        public const string Z0302 = "Z0302";
        public const string Z0303 = "Z0303";

        // --- Error messages
        private static readonly IDictionary<string, string> s_Messages = new Dictionary<string, string>
        {
            { Z0001, "The '{0}' operation with the specified operands is invalid" },
            { Z0002, "Bit index should be between 0 and 7. '{0}' is invalid" },
            { Z0004, "{0} operation cannot have {1} as its operand" },
            { Z0005, "'IN reg,(port)' operation can use only 'A' as its register operand" },
            { Z0006, "Output value can only be 0" },
            { Z0007, "The first 8-bit argument of {0} can only be 'A'" },
            { Z0008, "The second operand of '{0} A,{1}' is invalid" },
            { Z0009, "{0} cannot have {1} as its first operand" },
            { Z0010, "'{0} {1},...' cannot have {2} as its second operand" },
            { Z0011, "{0} cannot be used with {1}" },
            { Z0012, "The EX AF,... operation should use AF' as its second argument" },
            { Z0013, "The EX DE,... operation should use HL as its second argument" },
            { Z0014, "The EX operation should use AF, DE, or (SP) as its first argument" },
            { Z0015, "The EX (SP)' operation should use HL, IX, or IY as its second argument" },
            { Z0016, "JP can be used only with (HL), (IX), or (IY), but no other forms of indirection" },
            { Z0017, "JP with an indirect target cannot be used with conditions" },
            { Z0018, "RST can be used only with #00, #08, #10, #18, #20, #28, #30, or #38 arguments. #{0} is invalid." },
            { Z0019, "{0} AF' is invalid" },
            { Z0020, "Interrupt mode can only be 0, 1, or 2. '{0}' is invalid." },
            { Z0021, "'LD {0},{1}' is an invalid operation" },
            { Z0022, "Relative jump distance should be between -128 and 127. {0} is invalid" },
            { Z0023, "The first operand must be A when using the two-argument form of {0}" },
            { Z0040, "Label '{0}' is already defined" },
            { Z0060, "Unexpected #else directive" },
            { Z0061, "Unexpected #endif directive" },
            { Z0062, "Missing #endif directive" },
            { Z0080, "A pragma or an operation line was expected, but '{0}' line received" },
            { Z0081, "SKIP to {0} is invalid, as this address is less then the current address, {1}" },
            { Z0082, "An EQU pragma must have a label" },
            { Z0083, "Unexpected operation type '{0}'" },
            { Z0084, "No processing rules defined for '{0}' operation" },
            { Z0085, "Cannot find code for key {0} in operation '{1}'" },
            { Z0086, "A VAR pragma must have a label" },
            { Z0087, "A VAR pragma cannot redefine a non-VAR-created symbol" },
            { Z0088, "A MODEL pragma can be used only once." },
            { Z0089, "A MODEL pragma can have only these values: 'SPECTRUM48', 'SPECTRUM128', 'SPECTRUMP3', 'NEXT'." },
            { Z0090, "An #ifmod or #ifnmod directive cen be used only with these identifiers: 'SPECTRUM48', 'SPECTRUM128', 'SPECTRUMP3', 'NEXT'." },
            { Z0100, "Unexpected token: '{0}'" },
            { Z0101, "Unexpected end of line" },
            { Z0200, "Expression evaluation resulted an error: {0}" },
            { Z0201, "This expression cannot be evaluated promptly, it may refer to an undefined symbol" },
            { Z0300, "Cannot find include file: '{0}'" },
            { Z0301, "Error reading include file: '{0}' ({1})" },
            { Z0302, "Include file '{0}' is included more than once into the same parent source file" },
            { Z0303, "Include file '{0}' causes circular file reference" }
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