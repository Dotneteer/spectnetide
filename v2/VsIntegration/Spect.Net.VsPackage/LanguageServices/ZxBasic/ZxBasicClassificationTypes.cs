using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// This class defines the types for the ZX BASIC classification
    /// </summary>
    internal static class ZxBasicClassificationTypes
    {
        public const string ZXB_CONSOLE = "ZXB_Console";
        public const string ZXB_PREPROC = "ZXB_Preprocessor";
        public const string ZXB_STATEMENT = "ZXB_Statement";
        public const string ZXB_CONTROL_FLOW = "ZXB_ControlFlow";
        public const string ZXB_COMMENT = "ZXB_Comment";
        public const string ZXB_FUNCTION = "ZXB_Function";
        public const string ZXB_OPERATOR = "ZXB_Operator";
        public const string ZXB_TYPE = "ZXB_Type";
        public const string ZXB_IDENTIFIER = "ZXB_Identifier";
        public const string ZXB_NUMBER = "ZXB_Number";
        public const string ZXB_STRING = "ZXB_String";
        public const string ZXB_LABEL = "ZXB_Label";
        public const string ZXB_ASM = "ZXB_Asm";

        [Export]
        [Name(ZXB_CONSOLE)]
        public static ClassificationTypeDefinition Console { get; set; }

        [Export]
        [Name(ZXB_PREPROC)]
        public static ClassificationTypeDefinition Preprocessor { get; set; }

        [Export]
        [Name(ZXB_STATEMENT)]
        public static ClassificationTypeDefinition Statement { get; set; }

        [Export]
        [Name(ZXB_CONTROL_FLOW)]
        public static ClassificationTypeDefinition ControlFlow { get; set; }

        [Export]
        [Name(ZXB_COMMENT)]
        public static ClassificationTypeDefinition Comment { get; set; }

        [Export]
        [Name(ZXB_FUNCTION)]
        public static ClassificationTypeDefinition Function { get; set; }

        [Export]
        [Name(ZXB_OPERATOR)]
        public static ClassificationTypeDefinition Operator { get; set; }

        [Export]
        [Name(ZXB_TYPE)]
        public static ClassificationTypeDefinition Type { get; set; }

        [Export]
        [Name(ZXB_IDENTIFIER)]
        public static ClassificationTypeDefinition Identifier { get; set; }

        [Export]
        [Name(ZXB_NUMBER)]
        public static ClassificationTypeDefinition Number { get; set; }

        [Export]
        [Name(ZXB_STRING)]
        public static ClassificationTypeDefinition String { get; set; }

        [Export]
        [Name(ZXB_LABEL)]
        public static ClassificationTypeDefinition Label { get; set; }

        [Export]
        [Name(ZXB_ASM)]
        public static ClassificationTypeDefinition Asm { get; set; }
    }
}
