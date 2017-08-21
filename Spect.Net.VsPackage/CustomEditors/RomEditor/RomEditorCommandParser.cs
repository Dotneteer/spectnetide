using System.Text.RegularExpressions;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    public class RomEditorCommandParser: CommandParser<RomEditorCommandType>
    {
        private static readonly Regex s_GotoRegex = new Regex(@"^([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_DisassRegex = new Regex(@"^[#]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_ExitDisassRegex = new Regex(@"^[xX]\s*$");

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public RomEditorCommandParser(string commandText) : base(commandText)
        {
        }

        /// <summary>
        /// Parses the command
        /// </summary>
        /// <param name="commandText">Command text to parse</param>
        protected override void Parse(string commandText)
        {
            Arg1 = null;
            Arg2 = null;

            // --- Check for empty command
            if (string.IsNullOrWhiteSpace(commandText))
            {
                Command = RomEditorCommandType.None;
                return;
            }

            // --- Check for GOTO command
            var match = s_GotoRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.Goto;
                if (!GetLabel(match))
                {
                    Command = RomEditorCommandType.Invalid;
                }
                return;
            }

            // --- Check for DISASS command
            match = s_DisassRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.Disassemble;
                if (!GetLabel(match))
                {
                    Command = RomEditorCommandType.Invalid;
                }
                return;
            }

            // --- Check for EXIT DISASS command
            match = s_ExitDisassRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.ExitDisass;
                return;
            }

            // --- Do not accept any other command
            Command = RomEditorCommandType.Invalid;
        }
    }
}