using System.Globalization;
using System.Text.RegularExpressions;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This class is intended to by the bas class of simple command parsers
    /// </summary>
    public abstract class CommandParser<TCommandType>
    {
        /// <summary>
        /// Type of the command
        /// </summary>
        public TCommandType Command { get; protected set; }

        /// <summary>
        /// Command address
        /// </summary>
        public ushort Address { get; protected set; }

        /// <summary>
        /// Command address #2
        /// </summary>
        public ushort Address2 { get; protected set; }

        /// <summary>
        /// Command argument #1
        /// </summary>
        public string Arg1 { get; protected set; }

        /// <summary>
        /// Command argument #2
        /// </summary>
        public string Arg2 { get; protected set; }

        /// <summary>
        /// Command argument #3
        /// </summary>
        public string Arg3 { get; protected set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected CommandParser(string commandText)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Parse(commandText);
        }

        /// <summary>
        /// Parses the command
        /// </summary>
        /// <param name="commandText">Command text to parse</param>
        protected abstract void Parse(string commandText);

        /// <summary>
        /// Gets the label value from the specified match
        /// </summary>
        /// <param name="match">Match instance</param>
        /// <param name="groupId">Group to check for address capture</param>
        /// <returns></returns>
        protected virtual bool GetLabel(Match match, int groupId = 1)
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
}
