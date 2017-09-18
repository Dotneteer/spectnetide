using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class is responsible for managing error messages
    /// </summary>
    public static class ErrorMessage
    {
        private static readonly IDictionary<string, string> _messages = new Dictionary<string, string>
        {
            { "Z0001", "The '{0}' operation with the specified operands is invalid" },
            { "Z0002", "Bit index should be between 0 and 7. '{0}' is invalid" },
            { "Z0004", "{0} operation can have {1} as its operand" },
            { "Z0005", "'IN reg,(port)' operation can use only 'A' as its register operand" },
            { "Z0006", "Output value can only be 0" },
            { "Z0007", "The first 8-bit argument of {0} can only be 'A'" },
            { "Z0008", "The second operand of '{0} A,{1}' is invalid" },
            { "Z0009", "{0} cannot have {1} as its first operand" },
            { "Z0010", "'{0} {1},...' cannot have {2} as its second operand" },
            { "Z0011", "{0} cannot be used with {1}" },
            { "Z0012", "The EX AF,... operation should use AF' as its second argument" },
            { "Z0013", "The EX DE,... operation should use HL as its second argument" },
            { "Z0014", "The EX operation should use AF, DE, or (SP) as its first argument" },
            { "Z0015", "The EX (SP)' operation should use HL, IX, or IY as its second argument" },
            { "Z0016", "JP can be used only with (HL), (IX), or (IY), but no other forms of indirection" },
            { "Z0017", "JP with an indirect target cannot be used with conditions" },
            { "Z0018", "RST can be used only with #00, #08, #10, #18, #20, #28, #30, or #38 arguments. {0} is invalid." },
            { "Z0019", "POP AF' is invalid" },
            { "Z0020", "Interrupt mode can only be 0, 1, or 2. '{0}' is invalid." },
            { "Z0021", "'LD {0},{1}' is an invalid operation" },
            { "Z0022", "Relative jump distance should be between -128 and 127. {0} is invalid" },
            { "Z0040", "Label '{0}' is already defined" },
            { "Z0060", "Unexpected #else directive" },
            { "Z0061", "Unexpected #endif directive" },
            { "Z0080", "A pragma or an operation line was expected, but '{0}' line received" },
            { "Z0081", "SKIP to {0} is invalid, as this address is less then the current address, {1}" },
            { "Z0082", "An EQU pragma must have a label" },
            { "Z0083", "Unexpected operation type '{0}'" },
            { "Z0084", "No processing rules defined for '{0}' operation" },
            { "Z0085", "Cannot find code for key {0} in operation '{1}'" },
            { "Z0100", "Unexpected token: '{0}'" },
            { "Z0101", "Unexpected end of line" },
            { "Z0200", "Expression evaluation resulted an error: {0}" },
            { "Z0201", "This expression cannot be evaluated promptly, it may refer to an undefined symbol" },
        };

        public static string GetMessage(string errorCode, params object[] parameters)
        {
            return _messages.TryGetValue(errorCode, out var message) 
                ? string.Format(message, parameters) 
                : $"Undefined error message code {errorCode}";
        }
    }
}