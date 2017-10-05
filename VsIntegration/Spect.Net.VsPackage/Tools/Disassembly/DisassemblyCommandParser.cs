using System.Text.RegularExpressions;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class parses and executes commands coming from the disassembly command prompt
    /// </summary>
    public class DisassemblyCommandParser: CommandParser<DisassemblyCommandType>
    {
        private static readonly Regex s_GotoRegex = new Regex(@"^[gG]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_LabelRegex = new Regex(@"^[lL]\s*([a-fA-F0-9]{1,4})(\s+(.*))?$");
        private static readonly Regex s_CommentRegex = new Regex(@"^[cC]\s*([a-fA-F0-9]{1,4})(\s+(.*))?$");
        private static readonly Regex s_PrefixCommentRegex = new Regex(@"^[pP]\s*([a-fA-F0-9]{1,4})(\s+(.*))?$");
        private static readonly Regex s_SetBreakPointRegex = new Regex(@"^[sS][bB]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_ToggleBreakPointRegex = new Regex(@"^[tT][bB]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_RemoveBreakPointRegex = new Regex(@"^[rR][bB]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_EraseAllBreakPointRegex = new Regex(@"^[eE][bB]$");
        private static readonly Regex s_RetrieveRegex = new Regex(@"^[rR]([lL]|[cC]|[pP])\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_SectionRegex = new Regex(@"^[mM]([dD]|[bB]|[wW]|[sS]|[cC])\s*([a-fA-F0-9]{1,4})\s+([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_LiteralRegex = new Regex(@"^[dD]\s*([a-fA-F0-9]{1,4})(\s+(#|[_a-zA-Z][_a-zA-Z0-9]*)?)?$");

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DisassemblyCommandParser(string commandText): base(commandText)
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
                Command = DisassemblyCommandType.None;
                return;
            }

            // --- Check for GOTO command
            var match = s_GotoRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.Goto;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                return;
            }

            // --- Check for LABEL command
            match = s_LabelRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.Label;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                var group = match.Groups[3];
                Arg1 = group.Captures.Count >0 ? group.Captures[0].Value : null;
                return;
            }

            // --- Check for COMMENT command
            match = s_CommentRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.Comment;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                var group = match.Groups[3];
                Arg1 = group.Captures.Count > 0 ? group.Captures[0].Value : null;
                return;
            }

            // --- Check for PREFIX COMMENT command
            match = s_PrefixCommentRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.PrefixComment;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                var group = match.Groups[3];
                Arg1 = group.Captures.Count > 0 ? group.Captures[0].Value : null;
                return;
            }

            // --- Check for retrieve operations
            match = s_RetrieveRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.Retrieve;
                if (!GetLabel(match, 2))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                Arg1 = match.Groups[1].Captures[0].Value.ToLower();
                return;
            }

            // --- Check for memory section operations
            match = s_SectionRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.AddSection;
                if (!GetLabel(match, 2))
                {
                    Command = DisassemblyCommandType.Invalid;
                    return;
                }
                Address2 = Address;
                Arg1 = match.Groups[1].Captures[0].Value.ToLower();
                if (!GetLabel(match, 3))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                return;
            }

            // --- Check for DEFINE LITERAL command
            match = s_LiteralRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.Literal;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                var group = match.Groups[3];
                Arg1 = group.Captures.Count > 0 ? group.Captures[0].Value : null;
                return;
            }

            // --- Check for SET BREAKPOINT command
            match = s_SetBreakPointRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.SetBreakPoint;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                return;
            }

            // --- Check for TOGGLE BREAKPOINT command
            match = s_ToggleBreakPointRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.ToggleBreakPoint;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                return;
            }

            // --- Check for CLEAR BREAKPOINT command
            match = s_RemoveBreakPointRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.RemoveBreakPoint;
                if (!GetLabel(match))
                {
                    Command = DisassemblyCommandType.Invalid;
                }
                return;
            }

            // --- Check for CLEAR ALL BREAKPOINT command
            match = s_EraseAllBreakPointRegex.Match(commandText);
            if (match.Success)
            {
                Command = DisassemblyCommandType.EraseAllBreakPoint;
                return;
            }

            // --- Do not accept any other command
            Command = DisassemblyCommandType.Invalid;
        }
    }
}