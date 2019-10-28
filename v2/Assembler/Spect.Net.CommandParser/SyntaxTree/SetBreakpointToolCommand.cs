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
        /// SET BREAKPOINT address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// SET BREAKPOINT address
        /// </summary>
        public string Symbol { get; }

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
            if (context.LITERAL().Length < 1) return;
            var type = ProcessId(context.LITERAL()[0].GetText(), out var number, out var symbol);
            if (HasSemanticError) return;

            if (type)
            {
                Symbol = symbol;
            }
            else
            {
                Address = number;
            }
            if (HasSemanticError) return;
            var isCompact = !string.IsNullOrWhiteSpace(context.GetChild(1).GetText());

            if (context.ChildCount < (isCompact ? 3 : 4)) return;

            // --- We have hit and/or filter condition
            var hitChild = isCompact ? 4 : 5;
            var conditionChild = isCompact ? 5 : 6; 
            if (context.GetChild(isCompact ? 3 : 4).GetText().ToLower() == "h")
            {
                // --- Hit condition
                conditionChild = isCompact ? 11 : 12;
                HitConditionType = context.GetChild(hitChild).GetText();
                if (string.IsNullOrWhiteSpace(HitConditionType))
                {
                    hitChild = isCompact ? 5 : 6;
                    HitConditionType = context.GetChild(hitChild).GetText();
                }
                else
                {
                    conditionChild = isCompact ? 10 : 11;
                }
                hitChild++;
                if (!string.IsNullOrWhiteSpace(context.GetChild(hitChild).GetText()))
                {
                    conditionChild--;
                }
                if (context.LITERAL().Length > 1)
                {
                    HitConditionValue = ProcessNumber(context.LITERAL()[1].GetText());
                    if (HasSemanticError) return;
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