using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// Defines an editor format for a Z80 assembly label
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_LABEL)]
    [Name(Z80AsmClassificationTypes.Z80_LABEL)]
    [UserVisible(true)]
    internal sealed class Z80LabelClassifierFormat : ClassificationFormatDefinition
    {
        public Z80LabelClassifierFormat()
        {
            DisplayName = "Z80 Asm - Label";
            ForegroundColor = Colors.DarkOrange;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly pragma
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_PRAGMA)]
    [Name(Z80AsmClassificationTypes.Z80_PRAGMA)]
    [UserVisible(true)]
    internal sealed class Z80PragmaClassifierFormat : ClassificationFormatDefinition
    {
        public Z80PragmaClassifierFormat()
        {
            DisplayName = "Z80 Asm - Pragma";
            ForegroundColor = Colors.CadetBlue;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly directive
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_DIRECTIVE)]
    [Name(Z80AsmClassificationTypes.Z80_DIRECTIVE)]
    [UserVisible(true)]
    internal sealed class Z80DirectiveClassifierFormat : ClassificationFormatDefinition
    {
        public Z80DirectiveClassifierFormat()
        {
            DisplayName = "Z80 Asm - Directive";
            ForegroundColor = Colors.LightGray;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly include directive
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_INCLUDE_DIRECTIVE)]
    [Name(Z80AsmClassificationTypes.Z80_INCLUDE_DIRECTIVE)]
    [UserVisible(true)]
    internal sealed class Z80IncludeDirectiveClassifierFormat : ClassificationFormatDefinition
    {
        public Z80IncludeDirectiveClassifierFormat()
        {
            DisplayName = "Z80 Asm - Include directive";
            ForegroundColor = Colors.Silver;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly instruction
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_INSTRUCTION)]
    [Name(Z80AsmClassificationTypes.Z80_INSTRUCTION)]
    [UserVisible(true)]
    internal sealed class Z80InstructionClassifierFormat : ClassificationFormatDefinition
    {
        public Z80InstructionClassifierFormat()
        {
            DisplayName = "Z80 Asm - Instruction";
            ForegroundColor = Colors.NavajoWhite;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly comment
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_COMMENT)]
    [Name(Z80AsmClassificationTypes.Z80_COMMENT)]
    [UserVisible(true)]
    internal sealed class Z80CommentClassifierFormat : ClassificationFormatDefinition
    {
        public Z80CommentClassifierFormat()
        {
            DisplayName = "Z80 Asm - Comment";
            ForegroundColor = Colors.LimeGreen;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly number
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_NUMBER)]
    [Name(Z80AsmClassificationTypes.Z80_NUMBER)]
    [UserVisible(true)]
    internal sealed class Z80NumberClassifierFormat : ClassificationFormatDefinition
    {
        public Z80NumberClassifierFormat()
        {
            DisplayName = "Z80 Asm - Number";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly string
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_STRING)]
    [Name(Z80AsmClassificationTypes.Z80_STRING)]
    [UserVisible(true)]
    internal sealed class Z80StringClassifierFormat : ClassificationFormatDefinition
    {
        public Z80StringClassifierFormat()
        {
            DisplayName = "Z80 Asm - String";
            ForegroundColor = Colors.Cyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly identifier
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_IDENTIFIER)]
    [Name(Z80AsmClassificationTypes.Z80_IDENTIFIER)]
    [UserVisible(true)]
    internal sealed class Z80IdentifierClassifierFormat : ClassificationFormatDefinition
    {
        public Z80IdentifierClassifierFormat()
        {
            DisplayName = "Z80 Asm - Identifier";
            ForegroundColor = Colors.DarkCyan;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly function
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_FUNCTION)]
    [Name(Z80AsmClassificationTypes.Z80_FUNCTION)]
    [UserVisible(true)]
    internal sealed class Z80FunctionClassifierFormat : ClassificationFormatDefinition
    {
        public Z80FunctionClassifierFormat()
        {
            DisplayName = "Z80 Asm - Function";
            ForegroundColor = Colors.DarkCyan;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 breakpoint line
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_BREAKPOINT)]
    [Name(Z80AsmClassificationTypes.Z80_BREAKPOINT)]
    [UserVisible(true)]
    [Order(Before = Z80AsmClassificationTypes.Z80_CURRENT_BREAKPOINT)]
    internal sealed class Z80BreakpointClassifierFormat : ClassificationFormatDefinition
    {
        public Z80BreakpointClassifierFormat()
        {
            DisplayName = "Z80 Asm - Breakpoint";
            BackgroundColor = Colors.Red;
            BackgroundOpacity = 0.4;
        }
    }

    /// <summary>
    /// Defines an editor format for the current Z80 breakpoint line
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_CURRENT_BREAKPOINT)]
    [Name(Z80AsmClassificationTypes.Z80_CURRENT_BREAKPOINT)]
    [UserVisible(true)]
    [Order(After = Z80AsmClassificationTypes.Z80_BREAKPOINT)]
    internal sealed class Z80CurrentBreakpointClassifierFormat : ClassificationFormatDefinition
    {
        public Z80CurrentBreakpointClassifierFormat()
        {
            DisplayName = "Z80 Asm - Current breakpoint";
            BackgroundColor = Colors.Orange;

        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly macro parameter
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_MACRO_PARAM)]
    [Name(Z80AsmClassificationTypes.Z80_MACRO_PARAM)]
    [UserVisible(true)]
    internal sealed class Z80MacroParamFormat : ClassificationFormatDefinition
    {
        public Z80MacroParamFormat()
        {
            DisplayName = "Z80 Asm - Macro Parameter";
            ForegroundColor = Colors.DarkOrchid;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly statement
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_STATEMENT)]
    [Name(Z80AsmClassificationTypes.Z80_STATEMENT)]
    [UserVisible(true)]
    internal sealed class Z80StatementFormat : ClassificationFormatDefinition
    {
        public Z80StatementFormat()
        {
            DisplayName = "Z80 Asm - Statement";
            ForegroundColor = Colors.OliveDrab;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly module keyword
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_MODULE)]
    [Name(Z80AsmClassificationTypes.Z80_MODULE)]
    [UserVisible(true)]
    internal sealed class Z80ModuleFormat : ClassificationFormatDefinition
    {
        public Z80ModuleFormat()
        {
            DisplayName = "Z80 Asm - Module";
            ForegroundColor = Colors.Yellow;
            BackgroundColor = Color.FromArgb(255, 80, 80, 80);
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 assembly macro invocation
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_MACRO_INVOCATION)]
    [Name(Z80AsmClassificationTypes.Z80_MACRO_INVOCATION)]
    [UserVisible(true)]
    internal sealed class Z80MacroInvocationFormat : ClassificationFormatDefinition
    {
        public Z80MacroInvocationFormat()
        {
            DisplayName = "Z80 Asm - Invoke macro";
            ForegroundColor = Colors.OliveDrab;
            IsItalic = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 operand
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_OPERAND)]
    [Name(Z80AsmClassificationTypes.Z80_OPERAND)]
    [UserVisible(true)]
    internal sealed class Z80OperandClassifierFormat : ClassificationFormatDefinition
    {
        public Z80OperandClassifierFormat()
        {
            DisplayName = "Z80 Asm - Operand";
            ForegroundColor = Colors.NavajoWhite;
            IsBold = true;
        }
    }

    /// <summary>
    /// Defines an editor format for a Z80 operand
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Z80AsmClassificationTypes.Z80_SEMI_VAR)]
    [Name(Z80AsmClassificationTypes.Z80_SEMI_VAR)]
    [UserVisible(true)]
    internal sealed class Z80SemiVarClassifierFormat : ClassificationFormatDefinition
    {
        public Z80SemiVarClassifierFormat()
        {
            DisplayName = "Z80 Asm - Semi-variables";
            ForegroundColor = Colors.LightCoral;
            IsItalic = true;
        }
    }
}