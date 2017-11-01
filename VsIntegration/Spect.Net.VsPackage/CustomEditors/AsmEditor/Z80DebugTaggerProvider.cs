using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(Z80DebugTokenTag))]
    public class Z80DebugTaggerProvider: IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            return new Z80DebugTokenTagger(textView, buffer) as ITagger<T>;
        }
    }
}