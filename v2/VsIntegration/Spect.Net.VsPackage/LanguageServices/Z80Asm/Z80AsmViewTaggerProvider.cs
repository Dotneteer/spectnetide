using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Spect.Net.VsPackage.Debugging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// Tagger provider for the Z80 Assembly editor
    /// </summary>
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(Z80AsmLanguageService.LANGUAGE_NAME)]
    [TagType(typeof(TextMarkerTag))]
    public class Z80AsmViewTaggerProvider : IViewTaggerProvider
    {
        // --- We keep track of active classifiers
        private static Dictionary<string, Z80AsmViewTagger> s_ActiveTaggers =
            new Dictionary<string, Z80AsmViewTagger>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The service that maintains the collection of all known classification types.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry;

        /// <summary>
        /// Creates an ITagAggregator{T} for an ITextBuffer.
        /// </summary>
        [Import]
        internal IViewTagAggregatorFactoryService AggregatorFactory;

        /// <summary>
        /// Sites this classification provider with the package
        /// </summary>
        public static void AttachToPackage()
        {
            SpectNetPackage.Default.ApplicationObject.Events.WindowEvents.WindowClosing += OnWindowClosing;
        }

        /// <summary>
        /// Detaches this classification provider frod the package
        /// </summary>
        public static void DetachFromPackage()
        {
            try
            {
                SpectNetPackage.Default.ApplicationObject.Events.WindowEvents.WindowClosing -= OnWindowClosing; ;
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
        }

        /// <summary>
        /// Creates a tag provider for the specified view and buffer.
        /// </summary>
        /// <param name="textView">
        /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
        /// </param>
        /// <param name="buffer">
        /// The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.
        /// </param>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <returns>
        /// The <see cref="T:Microsoft.VisualStudio.Text.Tagging.ITagAggregator`1" /> 
        /// of the correct type for <paramref name="textView" />.
        /// </returns>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            string filePath = null;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument docProperty))
            {
                filePath = docProperty.FilePath;
            }
            var tagger = new Z80AsmViewTagger(buffer, textView, filePath);
            s_ActiveTaggers[filePath] = tagger;
            return tagger as ITagger<T>;
        }

        public static void UpdateBreakpointVisuals(string breakpointFile, int breakpointLine)
        {
            if (!s_ActiveTaggers.TryGetValue(breakpointFile, out var tagger))
            {
                return;
            }

            tagger.UpdateLine(breakpointLine);
        }

        /// <summary>
        /// Remove the matching classifier from the document list
        /// </summary>
        private static void OnWindowClosing(EnvDTE.Window Window)
        {
            ScanActiveClassifierWindows();
        }

        /// <summary>
        /// Scans the active classification windows
        /// </summary>
        private static void ScanActiveClassifierWindows()
        {
            var windows = SpectNetPackage.Default.ApplicationObject.Windows;
            var activeClassifiers = new Dictionary<string, Z80AsmViewTagger>(StringComparer.InvariantCultureIgnoreCase);

            // --- Collect active document names
            foreach (EnvDTE.Window window in windows)
            {
                var docName = window.Document?.FullName;
                if (docName != null && s_ActiveTaggers.TryGetValue(docName, out var classifier))
                {
                    activeClassifiers.Add(docName, classifier);
                }
            }
            s_ActiveTaggers = activeClassifiers;
        }
    }
}
