using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.LanguageServices.Z80Test
{
    /// <summary>
    /// This class defines the content type for the Z80 Unit Test language
    /// </summary>
    public class Z80TestContentTypeDefinition
    {
        [Export(typeof(ContentTypeDefinition))]
        [Name(Z80TestLanguageService.LANGUAGE_NAME)]
        [BaseDefinition("code")]
        public ContentTypeDefinition Z80TestContentType { get; set; }
    }
}