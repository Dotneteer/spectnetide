using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This class implements the language service that can handle the Z80 Assembly
    /// language.
    /// </summary>
    [Guid("DDE2DE44-3565-4DF0-A28F-3315A841191C")]
    public class Z80AsmLanguageService: LanguageService
    {
        public const string LANGUAGE_NAME = "Z80Assembler";

        /// <summary>
        /// Cache language preferences here
        /// </summary>
        private LanguagePreferences _preferences;

        public Z80AsmLanguageService(object site)
        {
            SetSite(site);
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(Site, typeof(Z80AsmLanguageService).GUID, Name);
                if (_preferences != null)
                {
                    _preferences.Init();

                    _preferences.EnableCodeSense = true;
                    _preferences.EnableMatchBraces = true;
                    _preferences.EnableMatchBracesAtCaret = true;
                    _preferences.EnableShowMatchingBrace = true;
                    _preferences.EnableCommenting = true;
                    _preferences.HighlightMatchingBraceFlags = _HighlightMatchingBraceFlags.HMB_USERECTANGLEBRACES;
                    _preferences.LineNumbers = false;
                    _preferences.MaxErrorMessages = 100;
                    _preferences.AutoOutlining = false;
                    _preferences.MaxRegionTime = 2000;
                    _preferences.InsertTabs = false;
                    _preferences.IndentSize = 2;
                    _preferences.IndentStyle = IndentingStyle.Smart;

                    _preferences.WordWrap = false;
                    _preferences.WordWrapGlyphs = false;

                    _preferences.AutoListMembers = true;
                    _preferences.EnableQuickInfo = true;
                    _preferences.ParameterInformation = true;
                }
            }

            return _preferences;
        }

        /// <summary>
        /// Signs that Z80Assembler does not use a scanner
        /// </summary>
        /// <param name="buffer">Text lines in the buffer</param>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return null;
        }

        /// <summary>
        /// Signs that we do not use a parse source
        /// </summary>
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return new Z80AssemblyAuthoringScope(req);
        }

        /// <summary>
        /// Retrieves the filter list for the language file
        /// </summary>
        public override string GetFormatFilterList()
        {
            return "Z80 Assembly File (*.z80asm)|*.z80asm";
        }

        /// <summary>
        /// Gets the name of the language
        /// </summary>
        public override string Name => LANGUAGE_NAME;

        public override void Dispose()
        {
            try
            {
                if (_preferences != null)
                {
                    _preferences.Dispose();
                    _preferences = null;
                }
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}