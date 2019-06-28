using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Test
{
    /// <summary>
    /// Defines an editor format for a Z80 test language keywords
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_KEYWORD)]
    [Name(Z80TestClassificationTypes.Z80_T_KEYWORD)]
    [UserVisible(true)]
    internal sealed class Z80TestKeywordClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestKeywordClassifierFormat()
        {
            DisplayName = "Z80 Test - Keyword";
            ForegroundColor = Colors.NavajoWhite;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 test language comment
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_COMMENT)]
    [Name(Z80TestClassificationTypes.Z80_T_COMMENT)]
    [UserVisible(true)]
    internal sealed class Z80TestCommentClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestCommentClassifierFormat()
        {
            DisplayName = "Z80 Test - Comment";
            ForegroundColor = Colors.LimeGreen;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 test language number
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_NUMBER)]
    [Name(Z80TestClassificationTypes.Z80_T_NUMBER)]
    [UserVisible(true)]
    internal sealed class Z80TestNumberClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestNumberClassifierFormat()
        {
            DisplayName = "Z80 Test - Number";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 test language identifier
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_IDENTIFIER)]
    [Name(Z80TestClassificationTypes.Z80_T_IDENTIFIER)]
    [UserVisible(true)]
    internal sealed class Z80TestIdentifierClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestIdentifierClassifierFormat()
        {
            DisplayName = "Z80 Test - Identifier";
            ForegroundColor = Colors.DarkCyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 test language Z80 name
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_KEY)]
    [Name(Z80TestClassificationTypes.Z80_T_KEY)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Z80TestKeyClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestKeyClassifierFormat()
        {
            DisplayName = "Z80 Test - Z80 Name";
            ForegroundColor = Colors.Orange;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 test breakpoint line
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_BREAKPOINT)]
    [Name(Z80TestClassificationTypes.Z80_T_BREAKPOINT)]
    [UserVisible(true)]
    internal sealed class Z80TestBreakpointClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestBreakpointClassifierFormat()
        {
            DisplayName = "Z80 Test - Breakpoint";
            BackgroundColor = Colors.DarkRed;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 test current breakpoint line
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80TestClassificationTypes.Z80_T_CURRENT_BREAKPOINT)]
    [Name(Z80TestClassificationTypes.Z80_T_CURRENT_BREAKPOINT)]
    [UserVisible(true)]
    [Order(After = Z80TestClassificationTypes.Z80_T_BREAKPOINT)]
    internal sealed class Z80TestCurrentBreakpointClassifierFormat : ClassificationFormatDefinition
    {
        public Z80TestCurrentBreakpointClassifierFormat()
        {
            DisplayName = "Z80 Test - Current breakpoint";
            BackgroundColor = Colors.OrangeRed;
        }
    }
}
