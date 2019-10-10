using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// This class defines the content type for the ZX BASIC language
    /// </summary>
    public class ZxBasicContentTypeDefinition
    {
        [Export(typeof(ContentTypeDefinition))]
        [Name(ZxBasicLanguageService.LANGUAGE_NAME)]
        [BaseDefinition("code")]
        public ContentTypeDefinition ZxBasicContentType { get; set; }
    }
}
