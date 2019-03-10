using Antlr4.Runtime.Tree;
using Spect.Net.CommandParser.Generated;
using Spect.Net.CommandParser.SyntaxTree;

namespace Spect.Net.CommandParser
{
    public class CommandToolVisitor: CommandToolBaseVisitor<object>
    {
        /// <summary>
        /// Visit a parse tree produced by <see cref="CommandToolParser.toolCommand"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitToolCommand(CommandToolParser.ToolCommandContext context)
        {
            if (context.gotoCommand() != null)
            {
                return new GotoToolCommand(context.gotoCommand());
            }

            if (context.romPageCommand() != null)
            {
                return new RomPageToolCommand(context.romPageCommand());
            }

            if (context.bankPageCommand() != null)
            {
                return new BankPageToolCommand(context.bankPageCommand());
            }

            if (context.memModeCommand() != null)
            {
                return new MemoryModeToolCommand();
            }

            if (context.labelCommand() != null)
            {
                return new LabelToolCommand(context.labelCommand());
            }

            if (context.commentCommand() != null)
            {
                return new CommentToolCommand(context.commentCommand());
            }

            if (context.prefixCommentCommand() != null)
            {
                return new PrefixCommentToolCommand(context.prefixCommentCommand());
            }

            if (context.setBreakpointCommand() != null)
            {
                return new SetBreakpointToolCommand(context.setBreakpointCommand());
            }

            if (context.toggleBreakpointCommand() != null)
            {
                return new ToggleBreakpointToolCommand(context.toggleBreakpointCommand());
            }

            if (context.removeBreakpointCommand() != null)
            {
                return new RemoveBreakpointToolCommand(context.removeBreakpointCommand());
            }

            if (context.updateBreakpointCommand() != null)
            {
                return new UpdateBreakpointToolCommand(context.updateBreakpointCommand());
            }

            if (context.eraseAllBreakpointsCommand() != null)
            {
                return new EraseAllBreakpointsToolCommand();
            }

            if (context.retrieveCommand() != null)
            {
                return new RetrieveToolCommand(context.retrieveCommand());
            }

            if (context.literalCommand() != null)
            {
                return new LiteralToolCommand(context.literalCommand());
            }

            if (context.disassemblyTypeCommand() != null)
            {
                return new DisassemblyTypeToolCommand(context.disassemblyTypeCommand());
            }

            if (context.reDisassemblyCommand() != null)
            {
                return new ReDisassemblyToolCommand();
            }

            if (context.jumpCommand() != null)
            {
                return new JumpToolCommand(context.jumpCommand());
            }

            if (context.sectionCommand() != null)
            {
                return new SectionToolCommand(context.sectionCommand());
            }

            if (context.compactCommand() != null)
            {
                return new CompactToolCommand(context.compactCommand());
            }

            if (context.compactCommand() != null)
            {
                return new CompactToolCommand(context.compactCommand());
            }

            if (context.addWatchCommand() != null)
            {
                return new AddWatchToolCommand(context.addWatchCommand());
            }

            if (context.removeWatchCommand() != null)
            {
                return new RemoveWatchToolCommand(context.removeWatchCommand());
            }

            if (context.updateWatchCommand() != null)
            {
                return new UpdateWatchToolCommand(context.updateWatchCommand());
            }

            if (context.labelWidthCommand() != null)
            {
                return new LabelWidthToolCommand(context.labelWidthCommand());
            }

            if (context.exchangeWatchCommand() != null)
            {
                return new ExchangeWatchToolCommand(context.exchangeWatchCommand());
            }

            if (context.eraseAllWatchCommand() != null)
            {
                return new EraseAllWatchToolCommand();
            }

            if (context.exportDisassemblyCommand() != null)
            {
                return new ExportDisassemblyToolCommand(context.exportDisassemblyCommand());
            }

            return null;
        }
    }
}