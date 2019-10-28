using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;

namespace Spect.Net.VsPackage.LanguageServices.Z80Test
{
    /// <summary>
    /// This class provides a classification for the Z80 Assembly language
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(Z80TestLanguageService.LANGUAGE_NAME)]
    public class Z80TestClassifierProvider : IClassifierProvider
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        [Import]
        private IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(()
                => new Z80TestClassifier(buffer, ClassificationRegistry));
        }
    }
}