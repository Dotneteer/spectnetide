using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.AsmEditor
{
    /// <summary>
    /// This type defines the classifications used by the Z80 assembler editor
    /// </summary>
    internal static class Z80AsmEditorClassificationDefinitions
    {
#pragma warning disable 169

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Label")]
        private static ClassificationTypeDefinition labelDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Pragma")]
        private static ClassificationTypeDefinition pragmaDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Directive")]
        private static ClassificationTypeDefinition directiveDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Instruction")]
        private static ClassificationTypeDefinition instructionDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Comment")]
        private static ClassificationTypeDefinition commentDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Number")]
        private static ClassificationTypeDefinition numberDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Identifier")]
        private static ClassificationTypeDefinition identifierDefinition;

#pragma warning restore 169
    }
}
