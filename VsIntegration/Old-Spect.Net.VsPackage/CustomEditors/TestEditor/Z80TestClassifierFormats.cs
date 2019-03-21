using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    public class Z80TestClassifierFormats
    {
        /// <summary>
        /// Defines an editor format for a Z80 test language keywords
        /// </summary>
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Z80TestKeyword")]
        [Name("Z80TestKeyword")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
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
        [ClassificationType(ClassificationTypeNames = "Z80TestComment")]
        [Name("Z80TestComment")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
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
        [ClassificationType(ClassificationTypeNames = "Z80TestNumber")]
        [Name("Z80TestNumber")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
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
        [ClassificationType(ClassificationTypeNames = "Z80TestIdentifier")]
        [Name("Z80TestIdentifier")]
        [UserVisible(true)]
        [Order(Before = Priority.Default)]
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
        [ClassificationType(ClassificationTypeNames = "Z80TestKey")]
        [Name("Z80TestKey")]
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
        [ClassificationType(ClassificationTypeNames = "Z80TestBreakpoint")]
        [Name("Z80TestBreakpoint")]
        [UserVisible(true)]
        [Order(After = Priority.Default)]
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
        [ClassificationType(ClassificationTypeNames = "Z80TestCurrentBreakpoint")]
        [Name("Z80TestCurrentBreakpoint")]
        [UserVisible(true)]
        [Order(Before = Priority.High)]
        internal sealed class Z80TestCurrentBreakpointClassifierFormat : ClassificationFormatDefinition
        {
            public Z80TestCurrentBreakpointClassifierFormat()
            {
                DisplayName = "Z80 Test - Current breakpoint";
                BackgroundColor = Colors.OrangeRed;
            }
        }
    }
}