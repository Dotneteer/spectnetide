using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
// ReSharper disable InconsistentNaming

#pragma warning disable 649

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// This type defines the classifications used by the Z80 assembler editor
    /// </summary>
    internal static class Z80AsmEditorClassificationDefinitions
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Label")]
        internal static ClassificationTypeDefinition labelDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Pragma")]
        internal static ClassificationTypeDefinition pragmaDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Directive")]
        internal static ClassificationTypeDefinition directiveDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80IncludeDirective")]
        internal static ClassificationTypeDefinition includeDirectiveDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Instruction")]
        internal static ClassificationTypeDefinition instructionDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Comment")]
        internal static ClassificationTypeDefinition commentDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Number")]
        internal static ClassificationTypeDefinition numberDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Identifier")]
        internal static ClassificationTypeDefinition identifierDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80String")]
        internal static ClassificationTypeDefinition stringDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Function")]
        internal static ClassificationTypeDefinition functionDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Breakpoint")]
        internal static ClassificationTypeDefinition breakpointDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80CurrentBreakpoint")]
        internal static ClassificationTypeDefinition currentBreakpointDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80MacroParam")]
        internal static ClassificationTypeDefinition macroParam;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Statement")]
        internal static ClassificationTypeDefinition statement;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80MacroInvocation")]
        internal static ClassificationTypeDefinition macroInvocation;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80Operand")]
        internal static ClassificationTypeDefinition register;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Z80SemiVar")]
        internal static ClassificationTypeDefinition semiVar;
    }
}

#pragma warning restore 649

