using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.AsmEditor
{
    /// <summary>
    /// Classifier provider for the Z80 Assembly editor
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("z80Asm")] 
    internal class Z80AsmEditorClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// The content type of the Z80 assembly editor
        /// </summary>
        [Export]
        [Name("z80Asm")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition Z80ContentType = null;

        /// <summary>
        /// We associate the Z80 assembly content type with the .z80Asm extension
        /// </summary>
        [Export]
        [FileExtension(".z80Asm")]
        [ContentType("z80Asm")]
        internal static FileExtensionToContentTypeDefinition OokFileType = null;


#pragma warning disable 649

        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

#pragma warning restore 649

        #region IClassifierProvider

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>
        /// A classifier for the text buffer, or null if the provider cannot do so 
        /// in its current state.
        /// </returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new Z80AsmEditorClassifier(_classificationRegistry));
        }

        #endregion
    }
}
