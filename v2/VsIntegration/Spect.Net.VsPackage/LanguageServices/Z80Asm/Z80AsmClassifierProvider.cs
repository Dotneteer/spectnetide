using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class provides a classification for the Z80 Assembly language
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(Z80AsmLanguageService.LANGUAGE_NAME)]
    public class Z80AsmClassifierProvider: IClassifierProvider
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        [Import]
        private IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() 
                => new Z80AsmClassifier(buffer, ClassificationRegistry));
        }
    }
}