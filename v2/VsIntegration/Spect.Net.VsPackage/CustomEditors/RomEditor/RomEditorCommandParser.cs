using System.Text.RegularExpressions;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    public class RomEditorCommandParser : CommandParser<RomEditorCommandType>
    {
        private static readonly Regex s_GotoRegex = new Regex(@"^([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_DisassRegex = new Regex(@"^[#]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_ExportDisassRegex = new Regex(@"^[xX]\s*([a-fA-F0-9]{1,4})\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_ExportProgramRegex = new Regex(@"^[pP]\s*$");
        private static readonly Regex s_ExitDisassRegex = new Regex(@"^[qQ]\s*$");
        private static readonly Regex s_SinclairModeRegex = new Regex(@"^[sS]\s*$");
        private static readonly Regex s_ZxBasicModeRegex = new Regex(@"^[zZ]\s*$");

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

            // --- Check for Sinclair Mode command
            match = s_SinclairModeRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.SinclairMode;
                return;
            }

            // --- Check for ZX BASIC Mode command
            match = s_ZxBasicModeRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.ZxBasicMode;
                return;
            }

            // --- Check for EXPORT PROGRAM command
            match = s_ExportProgramRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.ExportProgram;
                return;
            }

            // --- Check for EXPORT DISASSEMBLY command
            match = s_ExportDisassRegex.Match(commandText);
            if (match.Success)
            {
                Command = RomEditorCommandType.ExportDisass;
                if (!GetLabel(match))
                {
                    Command = RomEditorCommandType.Invalid;
                    return;
                }
                if (!GetLabel2(match))
                {
                    Command = RomEditorCommandType.Invalid;
                }
                return;
            }

            // --- Do not accept any other command
            Command = RomEditorCommandType.Invalid;
        }
    }
}
