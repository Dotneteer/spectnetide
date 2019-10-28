using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// Defines an editor format for a ZX BASIC console keyword
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_CONSOLE)]
    [Name(ZxBasicClassificationTypes.ZXB_CONSOLE)]
    [UserVisible(true)]
    internal sealed class ZxbConsoleClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbConsoleClassifierFormat()
        {
            DisplayName = "ZX Basic - Console";
            ForegroundColor = Color.FromArgb(255, 86, 156, 214);
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC preprocessor directive
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_PREPROC)]
    [Name(ZxBasicClassificationTypes.ZXB_PREPROC)]
    [UserVisible(true)]
    internal sealed class ZxbPreProcClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbPreProcClassifierFormat()
        {
            DisplayName = "ZX Basic - Preprocessor";
            ForegroundColor = Colors.LightGray;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC statement keyword
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_STATEMENT)]
    [Name(ZxBasicClassificationTypes.ZXB_STATEMENT)]
    [UserVisible(true)]
    internal sealed class ZxbStatementClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbStatementClassifierFormat()
        {
            DisplayName = "ZX Basic - Statement";
            ForegroundColor = Color.FromArgb(255, 86, 156, 214);
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC control flow statement keyword
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_CONTROL_FLOW)]
    [Name(ZxBasicClassificationTypes.ZXB_CONTROL_FLOW)]
    [UserVisible(true)]
    internal sealed class ZxbControlFlowClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbControlFlowClassifierFormat()
        {
            DisplayName = "ZX Basic - Control Flow";
            ForegroundColor = Color.FromArgb(255, 86, 156, 214);
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC comment
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_COMMENT)]
    [Name(ZxBasicClassificationTypes.ZXB_COMMENT)]
    [UserVisible(true)]
    internal sealed class ZxbCommentClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbCommentClassifierFormat()
        {
            DisplayName = "ZX Basic - Comment";
            ForegroundColor = Colors.Green;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC function
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_FUNCTION)]
    [Name(ZxBasicClassificationTypes.ZXB_FUNCTION)]
    [UserVisible(true)]
    internal sealed class ZxbFunctionClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbFunctionClassifierFormat()
        {
            DisplayName = "ZX Basic - Function";
            ForegroundColor = Colors.DarkCyan;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC operator
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_OPERATOR)]
    [Name(ZxBasicClassificationTypes.ZXB_OPERATOR)]
    [UserVisible(true)]
    internal sealed class ZxbOperatorClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbOperatorClassifierFormat()
        {
            DisplayName = "ZX Basic - Operator";
            ForegroundColor = Colors.DarkCyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC operator
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_TYPE)]
    [Name(ZxBasicClassificationTypes.ZXB_TYPE)]
    [UserVisible(true)]
    internal sealed class ZxbTypeClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbTypeClassifierFormat()
        {
            DisplayName = "ZX Basic - Type";
            ForegroundColor = Colors.DarkCyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC identifier
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_IDENTIFIER)]
    [Name(ZxBasicClassificationTypes.ZXB_IDENTIFIER)]
    [UserVisible(true)]
    internal sealed class ZxbIdentifierClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbIdentifierClassifierFormat()
        {
            DisplayName = "ZX Basic - Identifier";
            ForegroundColor = Colors.NavajoWhite;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC number
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_NUMBER)]
    [Name(ZxBasicClassificationTypes.ZXB_NUMBER)]
    [UserVisible(true)]
    internal sealed class ZxbNumberClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbNumberClassifierFormat()
        {
            DisplayName = "ZX Basic - Number";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC string
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_STRING)]
    [Name(ZxBasicClassificationTypes.ZXB_STRING)]
    [UserVisible(true)]
    internal sealed class ZxbStringClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbStringClassifierFormat()
        {
            DisplayName = "ZX Basic - String";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC label
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_STRING)]
    [Name(ZxBasicClassificationTypes.ZXB_LABEL)]
    [UserVisible(true)]
    internal sealed class ZxbLabelClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbLabelClassifierFormat()
        {
            DisplayName = "ZX Basic - Label";
            ForegroundColor = Colors.DarkOrange;
        }
    }

    /// <summary>
    /// Defines an editor format for a ZX BASIC ASM section
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ZxBasicClassificationTypes.ZXB_ASM)]
    [Name(ZxBasicClassificationTypes.ZXB_ASM)]
    [UserVisible(true)]
    internal sealed class ZxbAsmClassifierFormat : ClassificationFormatDefinition
    {
        public ZxbAsmClassifierFormat()
        {
            DisplayName = "ZX BASIC - Asm";
            ForegroundColor = Colors.OliveDrab;
        }
    }
}
