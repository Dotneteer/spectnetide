using System.Globalization;
using System.Text;
using Spect.Net.CommandParser.Generated;

namespace Spect.Net.CommandParser.SyntaxTree
{
    /// <summary>
    /// This class represents a SET BREAKPOINT tool command
    /// </summary>
    public class SetBreakpointToolCommand : ToolCommandNode
    {
        /// <summary>
        /// COMMENT address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Type of the optional HIT condition
        /// </summary>
        public string HitConditionType { get; }

        /// <summary>
        /// Value of the HIT condition
        /// </summary>
        public ushort HitConditionValue { get; }

        /// <summary>
        /// Value of the filter condition
        /// </summary>
        public string Condition { get; }

        public SetBreakpointToolCommand(CommandToolParser.SetBreakpointCommandContext context)
        {
            if (context.HEXNUM().Length > 0)
            {
                Address = ushort.Parse(context.HEXNUM()[0].GetText(), NumberStyles.HexNumber);
            }

            if (context.ChildCount < 4) return;

            // --- We have hit and/or filter condition
            var hitChild = 5;
            var conditionChild = 6; 
            if (context.GetChild(4).GetText().ToLower() == "h")
            {
                // --- Hit condition
                conditionChild = 12;
                HitConditionType = context.GetChild(hitChild).GetText();
                if (string.IsNullOrWhiteSpace(HitConditionType))
                {
                    hitChild = 6;
                    HitConditionType = context.GetChild(hitChild).GetText();
                }
                else
                {
                    conditionChild = 11;
                }
                hitChild++;
                if (!string.IsNullOrWhiteSpace(context.GetChild(hitChild).GetText()))
                {
                    conditionChild--;
                }
                if (context.HEXNUM().Length > 1)
                {
                    HitConditionValue = ushort.Parse(context.HEXNUM()[1].GetText(), NumberStyles.HexNumber);
                }
            }

            // --- The remaining part is the filter condition
            var sb = new StringBuilder();
            for (var i = conditionChild; i < context.ChildCount; i++)
            {
                sb.Append(context.GetChild(i).GetText());
            }
            Condition = sb.ToString();
        }
    }
}