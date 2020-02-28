using System.Collections.Generic;
// ReSharper disable StringLiteralTypo

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
        public const string Z0024 = "Z0024";
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
        public const string Z0091 = "Z0091";
        public const string Z0092 = "Z0092";
        public const string Z0093 = "Z0093";
        public const string Z0094 = "Z0094";

        public const string Z0100 = "Z0100";
        public const string Z0101 = "Z0101";
        public const string Z0102 = "Z0102";

        public const string Z0200 = "Z0200";
        public const string Z0201 = "Z0201";

        public const string Z0300 = "Z0300";
        public const string Z0301 = "Z0301";
        public const string Z0302 = "Z0302";
        public const string Z0303 = "Z0303";
        public const string Z0304 = "Z0304";
        public const string Z0305 = "Z0305";
        public const string Z0306 = "Z0306";
        public const string Z0307 = "Z0307";
        public const string Z0308 = "Z0308";
        public const string Z0309 = "Z0309";

        public const string Z0400 = "Z0400";
        public const string Z0401 = "Z0401";
        public const string Z0402 = "Z0402";
        public const string Z0403 = "Z0403";
        public const string Z0404 = "Z0404";
        public const string Z0405 = "Z0405";
        public const string Z0406 = "Z0406";
        public const string Z0407 = "Z0407";
        public const string Z0408 = "Z0408";
        public const string Z0409 = "Z0409";
        public const string Z0410 = "Z0410";
        public const string Z0411 = "Z0411";
        public const string Z0412 = "Z0412";
        public const string Z0413 = "Z0413";
        public const string Z0414 = "Z0414";
        public const string Z0415 = "Z0415";
        public const string Z0416 = "Z0416";
        public const string Z0417 = "Z0417";
        public const string Z0418 = "Z0418";
        public const string Z0419 = "Z0419";
        public const string Z0420 = "Z0420";
        public const string Z0421 = "Z0421";
        public const string Z0422 = "Z0422";
        public const string Z0423 = "Z0423";
        public const string Z0424 = "Z0424";
        public const string Z0425 = "Z0425";
        public const string Z0426 = "Z0426";
        public const string Z0427 = "Z0427";
        public const string Z0428 = "Z0428";
        public const string Z0429 = "Z0429";
        public const string Z0430 = "Z0430";
        public const string Z0431 = "Z0431";
        public const string Z0432 = "Z0432";
        public const string Z0433 = "Z0433";
        public const string Z0434 = "Z0434";
        public const string Z0435 = "Z0435";
        public const string Z0436 = "Z0436";
        public const string Z0437 = "Z0437";
        public const string Z0438 = "Z0438";
        public const string Z0439 = "Z0439";
        public const string Z0440 = "Z0440";
        public const string Z0441 = "Z0441";
        public const string Z0442 = "Z0442";
        public const string Z0443 = "Z0443";
        public const string Z0444 = "Z0444";
        public const string Z0445 = "Z0445";
        public const string Z0446 = "Z0446";
        public const string Z0447 = "Z0447";
        public const string Z0448 = "Z0448";
        public const string Z0449 = "Z0449";
        public const string Z0450 = "Z0450";
        public const string Z0451 = "Z0451";
        public const string Z0452 = "Z0452";
        public const string Z0453 = "Z0453";
        public const string Z0454 = "Z0454";

        public const string Z0500 = "Z0500";

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
            { Z0024, "POP cannot be used with an expression operand" },
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
            { Z0091, "DEFM/DEFN pragma requires a string argument." },
            { Z0092, "ALIGN pragma must be used with a parameter value between 1 and #4000; {0} in an invalid value." },
            { Z0093, "DEFH pragma requires a string argument." },
            { Z0094, "DEFH pragma requires a string with even hexadecimal digits." },
            { Z0100, "Unexpected token: '{0}'" },
            { Z0101, "Unexpected end of line" },
            { Z0102, "To use this Spectrum Next-specific instruction, you need to set MODEL type to NEXT explicitly." },
            { Z0200, "Expression evaluation resulted an error: {0}" },
            { Z0201, "This expression cannot be evaluated promptly, it may refer to one or more undefined symbols ({0})" },
            { Z0300, "Cannot find include file: '{0}'" },
            { Z0301, "Error reading include file: '{0}' ({1})" },
            { Z0302, "Include file '{0}' is included more than once into the same parent source file" },
            { Z0303, "Include file '{0}' causes circular file reference" },
            { Z0304, "The current assembly address overflew #FFFF" },
            { Z0305, "A string value is used where a numeric value is expected." },
            { Z0306, "A string value is expected." },
            { Z0307, "Cannot use an empty pattern with DEFG pragma." },
            { Z0308, "An integral value is expected." },
            { Z0309, "The emitted code overflows the segment/bank." },
            { Z0400, "You cannot define a macro without a name." },
            { Z0401, "Missing {0} statement." },
            { Z0402, "Macro name '{0}' has already been declared." },
            { Z0403, "Unknown macro argument is used '{0}' macro definition." },
            { Z0404, "Macro definition cannot be nested into another macro definition." },
            { Z0405, "Orphan '{0}' statement found without a corresponding '{1}' statement." },
            { Z0406, "Loop counter cannot be greater than 65535 (#FFFF)." },
            { Z0407, "The {0} pragma can be used only in the global scope." },
            { Z0408, "Too many errors detected while compiling a loop, further processing aborted." },
            { Z0409, "Loop counter exceeded the maximum value of 65535 (#FFFF)." },
            { Z0410, "IF cannot have an {0} section after a detected ELSE section." },
            { Z0411, "{0} section in IF cannot have a label." },
            { Z0412, "$CNT cannot be used outside of loop constructs." },
            { Z0413, "The STEP value in a FOR-loop cannot be zero." },
            { Z0414, "Variable {0} is already declared, it cannot be used as a FOR-loop variable again." },
            { Z0415, "BREAK cannot be used outside of loop constructs." },
            { Z0416, "CONTINUE cannot be used outside of loop constructs." },
            { Z0417, "Duplicated MACRO argument: {0}." },
            { Z0418, "Unknown MACRO: {0}." },
            { Z0419, "The declaration of MACRO {0} contains {1} argument(s), but it is invoked with more parameters ({2})." },
            { Z0420, "Macro parameter can only be used within a macro declaration." },
            { Z0421, "A parse-time function accepts only macro parameters." },
            { Z0422, "Cannot pass a macro parameter template in a macro parameter." },
            { Z0423, "Cannot open file '{0}' used in INCLUDEBIN pragma ({0})." },
            { Z0424, "Invalid INCLUDEBIN offset value (negative, or greater than the file length)." },
            { Z0425, "Invalid INCLUDEBIN length value (negative, or segment exceends the file length)." },
            { Z0426, "Emitting the INCLUDEBIN segment would overflow the #FFFF assembly address." },
            { Z0427, "You cannot define a macro with a temporary name ({0})." },
            { Z0428, "You cannot define a module without a name." },
            { Z0429, "Module with name '{0}' already exists." },
            { Z0430, "You cannot define a module with a temporary name ({0})." },
            { Z0431, "Only one XORG pragma can be used within a code segment." },
            { Z0432, "You cannot define a struct without a name." },
            { Z0433, "You cannot define a struct with a temporary name ({0})." },
            { Z0434, "Structure name '{0}' has already been declared." },
            { Z0435, "Structures can use only pragmas that emit bytes, words, strings, or reserve space." },
            { Z0436, "The ENDS statement cannot have a label." },
            { Z0437, "Too many errors withing a .struct definition." },
            { Z0438, "Duplicated field label {0} in a .struct definition." },
            { Z0439, "A .struct invocation ({0}) cannot have arguments." },
            { Z0440, "Field assignment instruction cannot be used outside of .struct invocation." },
            { Z0441, "The .struct definition of {0} does not have a field named {1}." },
            { Z0442, "The .struct size of {0} is {1} byte(s). The invocation wants to emit {2} bytes." },
            { Z0443, "Invalid COMPAREBIN offset value (negative, or greater than the file length)." },
            { Z0444, "Invalid COMPAREBIN length value (negative, or segment exceends the file length)." },
            { Z0445, "COMPAREBIN fails: {0}." },
            { Z0446, "Cannot open file '{0}' used in COMPAREBIN pragma ({0})." },
            { Z0447, "You cannot define a local symbol with a temporary name ({0})." },
            { Z0448, "LOCAL can be used only within PROC." },
            { Z0449, "This local symbol is already declared: ({0})." },
            { Z0450, "The .ZXBASIC pragma should be used before any other pragma or instruction." },
            { Z0451, "The .BANK pragma cannot have a label." },
            { Z0452, "The .BANK pragma cannot be used with the ZX Spectrum 48 model type." },
            { Z0453, "The .BANK pragma's value must be between 0 and 7." },
            { Z0454, "You have already used the .BANK pragma for bank {0}." },
            { Z0500, "ERROR: {0}" },
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