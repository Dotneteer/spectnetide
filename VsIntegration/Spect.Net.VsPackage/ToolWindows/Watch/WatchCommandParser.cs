using System.Text.RegularExpressions;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    public class WatchCommandParser: CommandParser<WatchCommandType>
    {
        private static readonly Regex s_AddRegex= new Regex(@"^[+]\s*(.+)$");
        private static readonly Regex s_RemoveRegex = new Regex(@"^[-]\s*([0-9]{1,3})\s*$");
        private static readonly Regex s_UpdateRegex = new Regex(@"^[*]\s*([0-9]{1,3})\s*(.+)$");
        private static readonly Regex s_LabelWidthRegex = new Regex(@"^[lL]\s*([0-9]{1,3})\s*$");
        private static readonly Regex s_MoveItemRegex = new Regex(@"^[xX]\s*([0-9]{1,3})\s+([0-9]{1,3})\s*$");
        private static readonly Regex s_EraseAllRegex = new Regex(@"^[eE]\s*$");

        /// <summary>
        /// Initializes a new instance of the command parser.</summary>
        public WatchCommandParser(string commandText) : base(commandText)
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
                Command = WatchCommandType.None;
                return;
            }

            var match = s_AddRegex.Match(commandText);
            if (match.Success)
            {
                Command = WatchCommandType.AddWatch;
                Arg1 = match.Groups[1].Captures[0].Value;
                return;
            }

            match = s_RemoveRegex.Match(commandText);
            if (match.Success)
            {
                Command = WatchCommandType.RemoveWatch;
                var num = GetUshort(match);
                if (num != null)
                {
                    Address = num.Value;
                    return;
                }
                Command = WatchCommandType.Invalid;
                return;
            }

            match = s_UpdateRegex.Match(commandText);
            if (match.Success)
            {
                Command = WatchCommandType.UpdateWatch;
                var num = GetUshort(match);
                if (num != null)
                {
                    Address = num.Value;
                    Arg1 = match.Groups[2].Captures[0].Value;
                    return;
                }
                Command = WatchCommandType.Invalid;
                return;
            }

            match = s_LabelWidthRegex.Match(commandText);
            if (match.Success)
            {
                Command = WatchCommandType.ChangeLabelWidth;
                var num = GetUshort(match);
                if (num != null)
                {
                    Address = num.Value;
                    return;
                }
                Command = WatchCommandType.Invalid;
                return;
            }

            match = s_MoveItemRegex.Match(commandText);
            if (match.Success)
            {
                Command = WatchCommandType.MoveItem;
                var num1 = GetUshort(match);
                if (num1 != null)
                {
                    Address = num1.Value;
                    var num2 = GetUshort(match, 2);
                    if (num2 != null)
                    {
                        Address2 = num2.Value;
                        return;
                    }
                }
                Command = WatchCommandType.Invalid;
                return;
            }

            match = s_EraseAllRegex.Match(commandText);
            if (match.Success)
            {
                Command = WatchCommandType.EraseAll;
                return;
            }

            // --- Do not accept any other command
            Command = WatchCommandType.Invalid;

        }

        /// <summary>
        /// Gets the label value from the specified match
        /// </summary>
        /// <param name="match">Match instance</param>
        /// <param name="groupId">Group to check for address capture</param>
        /// <returns></returns>
        private ushort? GetUshort(Match match, int groupId = 1)
        {
            var num = match.Groups[groupId].Captures[0].Value;
            if (!ushort.TryParse(num, out var value))
            {
                return null;
            }
            return value;
        }

    }
}