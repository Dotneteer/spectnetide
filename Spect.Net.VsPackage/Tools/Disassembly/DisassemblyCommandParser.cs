using System.Globalization;
using System.Text.RegularExpressions;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class parses and executes commands coming from the disassembly command prompt
    /// </summary>
    public class DisassemblyCommandParser
    {
        private static readonly Regex s_GotoRegex = new Regex(@"^[gG]\s*([a-zA-Z0-9]{1,4})$");
        private static readonly Regex s_LabelRegex = new Regex(@"^[lL]\s*([a-zA-Z0-9]{1,4})(\s+(.*))?$");
        private static readonly Regex s_CommentRegex = new Regex(@"^[cC]\s*([a-zA-Z0-9]{1,4})(\s+(.*))?$");
        private static readonly Regex s_PrefixCommentRegex = new Regex(@"^[pP]\s*([a-zA-Z0-9]{1,4})(\s+(.*))?$");
        private static readonly Regex s_SetBreakPointRegex = new Regex(@"^[sS][bB]\s*([a-zA-Z0-9]{1,4})$");
        private static readonly Regex s_ToggleBreakPointRegex = new Regex(@"^[tT][bB]\s*([a-zA-Z0-9]{1,4})$");
        private static readonly Regex s_RemoveBreakPointRegex = new Regex(@"^[rR][bB]\s*([a-zA-Z0-9]{1,4})$");
        private static readonly Regex s_EraseAllBreakPointRegex = new Regex(@"^[eE][bB]$");
        private static readonly Regex s_RetrieveRegex = new Regex(@"^[rR]([lL]|[cC]|[pP])\s*([a-zA-Z0-9]{1,4})$");
        private static readonly Regex s_SectionRegex = new Regex(@"^[mM]([dD]|[bB]|[wW]|[sS])\s*([a-zA-Z0-9]{1,4})\s+([a-zA-Z0-9]{1,4})$");
        private static readonly Regex s_LiteralRegex = new Regex(@"^[dD]\s*([a-zA-Z0-9]{1,4})(\s+([_a-zA-Z][_a-zA-Z0-9]*)?)?$");

        /// <summary>
        /// Type of the command
        /// </summary>
        public DisassemblyCommandType Command { get; private set; }

        /// <summary>
        /// Command address
        /// </summary>
        public ushort Address { get; private set; }

        /// <summary>
        /// Command argument #1
        /// </summary>
        public string Arg1 { get; private set; }

        /// <summary>
        /// Command address #2
        /// </summary>
        public ushort Address2 { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DisassemblyCommandParser(string commandText)
        {
            Parse(commandText);
        }

        /// <summary>
        /// Parses the command
        /// </summary>
        /// <param name="commandText"></param>
        private void Parse(string commandText)
        {
            Arg1 = null;

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

        private bool GetLabel(Match match, int groupId = 1)
        {
            var addrStr = match.Groups[groupId].Captures[0].Value;
            if (!int.TryParse(addrStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out int address))
            {
                return false;
            }
            Address = (ushort)address;
            return true;
        }
    }

    /// <summary>
    /// Command types available in the Disassembly tool window
    /// </summary>
    public enum DisassemblyCommandType
    {
        None,
        Invalid,
        Goto,
        Label,
        Comment,
        PrefixComment,
        SetBreakPoint,
        ToggleBreakPoint,
        RemoveBreakPoint,
        EraseAllBreakPoint,
        Retrieve,
        AddSection,
        Literal
    }
}