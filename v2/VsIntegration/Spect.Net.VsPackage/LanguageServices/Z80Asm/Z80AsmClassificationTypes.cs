using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class defines the types for the Z80 Assembler classification
    /// </summary>
    internal static class Z80AsmClassificationTypes
    {
        public const string Z80_LABEL = "Z80_Label";
        public const string Z80_PRAGMA = "Z80_Pragma";
        public const string Z80_DIRECTIVE = "Z80_Directive";
        public const string Z80_INCLUDE_DIRECTIVE = "Z80_IncludeDirective";
        public const string Z80_INSTRUCTION = "Z80_Instruction";
        public const string Z80_COMMENT = "Z80_Comment";
        public const string Z80_NUMBER = "Z80_Number";
        public const string Z80_IDENTIFIER = "Z80_Identifier";
        public const string Z80_STRING = "Z80_String";
        public const string Z80_FUNCTION = "Z80_Function";
        public const string Z80_MACRO_PARAM = "Z80_MacroParam";
        public const string Z80_STATEMENT = "Z80_Statement";
        public const string Z80_MACRO_INVOCATION = "Z80_MacroInvocation";
        public const string Z80_OPERAND = "Z80_Operand";
        public const string Z80_SEMI_VAR = "Z80_SemiVar";
        public const string Z80_MODULE = "Z80_Module";
        public const string Z80_BREAKPOINT = "Z80_Breakpoint";
        public const string Z80_CURRENT_BREAKPOINT = "Z80_CurrentBreakpoint";

        [Export]
        [Name(Z80_LABEL)]
        public static ClassificationTypeDefinition Label { get; set; }

        [Export]
        [Name(Z80_PRAGMA)]
        public static ClassificationTypeDefinition Pragma { get; set; }

        [Export]
        [Name(Z80_DIRECTIVE)]
        public static ClassificationTypeDefinition Directive { get; set; }

        [Export]
        [Name(Z80_INCLUDE_DIRECTIVE)]
        public static ClassificationTypeDefinition IncludeDirective { get; set; }

        [Export]
        [Name(Z80_INSTRUCTION)]
        public static ClassificationTypeDefinition Instruction { get; set; }

        [Export]
        [Name(Z80_COMMENT)]
        public static ClassificationTypeDefinition Comment { get; set; }

        [Export]
        [Name(Z80_NUMBER)]
        public static ClassificationTypeDefinition Number { get; set; }

        [Export]
        [Name(Z80_IDENTIFIER)]
        public static ClassificationTypeDefinition Identifier { get; set; }

        [Export]
        [Name(Z80_STRING)]
        public static ClassificationTypeDefinition String { get; set; }

        [Export]
        [Name(Z80_FUNCTION)]
        public static ClassificationTypeDefinition Function { get; set; }

        [Export]
        [Name(Z80_MACRO_PARAM)]
        public static ClassificationTypeDefinition MacroParam { get; set; }

        [Export]
        [Name(Z80_STATEMENT)]
        public static ClassificationTypeDefinition Statement { get; set; }

        [Export]
        [Name(Z80_MACRO_INVOCATION)]
        public static ClassificationTypeDefinition MacroInvocation { get; set; }

        [Export]
        [Name(Z80_OPERAND)]
        public static ClassificationTypeDefinition Operand { get; set; }

        [Export]
        [Name(Z80_SEMI_VAR)]
        public static ClassificationTypeDefinition SemiVar { get; set; }

        [Export]
        [Name(Z80_MODULE)]
        public static ClassificationTypeDefinition Module { get; set; }

        [Export]
        [Name(Z80_BREAKPOINT)]
        public static ClassificationTypeDefinition Breakpoint { get; set; }

        [Export]
        [Name(Z80_CURRENT_BREAKPOINT)]
        public static ClassificationTypeDefinition CurrentBreakpoint { get; set; }
    }
}