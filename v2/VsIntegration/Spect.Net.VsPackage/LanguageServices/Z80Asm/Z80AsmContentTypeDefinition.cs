using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class defines the content type for the Z80 Assembly language
    /// </summary>
    public class Z80AsmContentTypeDefinition
    {
        [Export(typeof(ContentTypeDefinition))]
        [Name(Z80AsmLanguageService.LANGUAGE_NAME)]
        [BaseDefinition("code")]
        public ContentTypeDefinition Z80AsmContentType { get; set; }
    }
}