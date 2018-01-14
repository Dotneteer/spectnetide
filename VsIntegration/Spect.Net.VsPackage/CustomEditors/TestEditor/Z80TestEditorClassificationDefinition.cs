using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    public class Z80TestEditorClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestKeyword")]
        internal static ClassificationTypeDefinition keywordDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestComment")]
        internal static ClassificationTypeDefinition commentDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestNumber")]
        internal static ClassificationTypeDefinition numberDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestIdentifier")]
        internal static ClassificationTypeDefinition identifierDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestKey")]
        internal static ClassificationTypeDefinition z80KeyDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestBreakpoint")]
        internal static ClassificationTypeDefinition breakpointDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80TestCurrentBreakpoint")]
        internal static ClassificationTypeDefinition currentBreakpointDefinition;
    }
}

#pragma warning restore 649
