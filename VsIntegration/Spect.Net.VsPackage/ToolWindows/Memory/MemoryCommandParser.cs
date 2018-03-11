using System.Text.RegularExpressions;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This class parses and executes commands coming from the memory command prompt
    /// </summary>
    public class MemoryCommandParser : CommandParser<MemoryCommandType>
    {
        private static readonly Regex s_GotoRegex = new Regex(@"^[gG]\s*([a-fA-F0-9]{1,4})$");
        private static readonly Regex s_GotoSymbolRegex = new Regex(@"^[gG][sS]?\s*([_a-zA-Z0-9]+)$");
        private static readonly Regex s_RomPageRegex = new Regex(@"^[rR]\s*([0-7])$");
        private static readonly Regex s_RamBankRegex = new Regex(@"^[bB]\s*([0-7])$");
        private static readonly Regex s_MemModeRegex = new Regex(@"^[mM]$");

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public MemoryCommandParser(string commandText) : base(commandText)
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
                Command = MemoryCommandType.None;
                return;
            }

            // --- Check for GOTO command
            var match = s_GotoRegex.Match(commandText);
            if (match.Success)
            {
                Command = MemoryCommandType.Goto;
                if (!GetLabel(match))
                {
                    Command = MemoryCommandType.Invalid;
                }
                return;
            }

            // --- Check for GOTO SYMBOL command
            match = s_GotoSymbolRegex.Match(commandText);
            if (match.Success)
            {
                Command = MemoryCommandType.GotoSymbol;
                Arg1 = match.Groups[1].Captures[0].Value;
                return;
            }

            // --- Check for ROM command
            match = s_RomPageRegex.Match(commandText);
            if (match.Success)
            {
                Command = MemoryCommandType.SetRomPage;
                if (!GetLabel(match))
                {
                    Command = MemoryCommandType.Invalid;
                }
                return;
            }

            // --- Check for RAM Bank command
            match = s_RamBankRegex.Match(commandText);
            if (match.Success)
            {
                Command = MemoryCommandType.SetRamBank;
                if (!GetLabel(match))
                {
                    Command = MemoryCommandType.Invalid;
                }
                return;
            }

            // --- Check for Memory mode command
            match = s_MemModeRegex.Match(commandText);
            if (match.Success)
            {
                Command = MemoryCommandType.MemoryMode;
                return;
            }

            // --- Do not accept any other command
            Command = MemoryCommandType.Invalid;
        }
    }
}