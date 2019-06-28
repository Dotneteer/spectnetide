using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Test
{
    /// <summary>
    /// This class defines the types for the Z80 Unit Test classification
    /// </summary>
    internal static class Z80TestClassificationTypes
    {
        public const string Z80_T_KEYWORD = "Z80T_Keyword";
        public const string Z80_T_COMMENT = "Z80T_Comment";
        public const string Z80_T_NUMBER = "Z80T_Number";
        public const string Z80_T_IDENTIFIER = "Z80T_Identifier";
        public const string Z80_T_KEY = "Z80T_Key";
        public const string Z80_T_BREAKPOINT = "Z80T_Breakpoint";
        public const string Z80_T_CURRENT_BREAKPOINT = "Z80T_Current_Breakpoint";

        [Export]
        [Name(Z80_T_KEYWORD)]
        public static ClassificationTypeDefinition Keyword { get; set; }

        [Export]
        [Name(Z80_T_COMMENT)]
        public static ClassificationTypeDefinition Comment { get; set; }

        [Export]
        [Name(Z80_T_NUMBER)]
        public static ClassificationTypeDefinition Number { get; set; }

        [Export]
        [Name(Z80_T_IDENTIFIER)]
        public static ClassificationTypeDefinition Identifier { get; set; }

        [Export]
        [Name(Z80_T_KEY)]
        public static ClassificationTypeDefinition Key { get; set; }

        [Export]
        [Name(Z80_T_BREAKPOINT)]
        public static ClassificationTypeDefinition Breakpoint { get; set; }

        [Export]
        [Name(Z80_T_CURRENT_BREAKPOINT)]
        public static ClassificationTypeDefinition CurrentBreakpoint { get; set; }
    }
}