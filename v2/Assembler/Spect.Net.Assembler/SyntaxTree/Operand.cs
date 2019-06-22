using Antlr4.Runtime;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class defines an operand for operations with compound addressing scheme
    /// </summary>
    public class Operand
    {
        /// <summary>
        /// The type of the operand
        /// </summary>
        public OperandType Type { get; set; } = OperandType.None;

        /// <summary>
        /// Name of the register (null in case of AddressIndirection and Expression)
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// The expression to evaluate (Expression, AddressIndirection, 
        /// and IndexedAddress)
        /// </summary>
        public ExpressionNode Expression { get; set; }

        /// <summary>
        /// Displacement sign in case of IndexedAddress, or null, 
        /// if there's no displacement
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Condition value
        /// </summary>
        public string Condition { get; set; }

        public Operand()
        {
            Type = OperandType.None;
        }

        public Operand(IZ80AsmVisitorContext visitorContext, Z80AsmParser.OperandContext context)
        {
            // --- The context has exactly one child
            ParserRuleContext regContext = null;
            if (context.reg8() != null)
            {
                Type = OperandType.Reg8;
                Register = context.reg8().NormalizeToken();
                regContext = context.reg8();
            }
            else if (context.reg8Idx() != null)
            {
                Type = OperandType.Reg8Idx;
                Register = context.reg8Idx().NormalizeToken();
                regContext = context.reg8Idx();
            }
            else if (context.reg8Spec() != null)
            {
                Type = OperandType.Reg8Spec;
                Register = context.reg8Spec().NormalizeToken();
                regContext = context.reg8Spec();
            }
            else if (context.reg16() != null)
            {
                Type = OperandType.Reg16;
                Register = context.reg16().NormalizeToken();
                regContext = context.reg16();
            }
            else if (context.reg16Idx() != null)
            {
                Type = OperandType.Reg16Idx;
                Register = context.reg16Idx().NormalizeToken();
                regContext = context.reg16Idx();
            }
            else if (context.reg16Spec() != null)
            {
                Type = OperandType.Reg16Spec;
                Register = context.reg16Spec().NormalizeToken();
                regContext = context.reg16Spec();
            }
            else if (context.memIndirect() != null)
            {
                var miContext = context.memIndirect();
                var expContext = miContext.expr();
                Type = OperandType.MemIndirect;
                Expression = visitorContext.GetExpression(expContext);
                if (miContext.LPAR() != null)
                {
                    visitorContext.AddOperand(miContext.LPAR());
                }
                if (miContext.RPAR() != null)
                {
                    visitorContext.AddOperand(miContext.RPAR());
                }
            }
            else if (context.regIndirect() != null)
            {
                Type = OperandType.RegIndirect;
                Register = context.regIndirect().NormalizeToken();
                regContext = context.regIndirect();
            }
            else if (context.cPort() != null)
            {
                Type = OperandType.CPort;
                regContext = context.cPort();
            }
            else if (context.indexedAddr() != null)
            {
                Type = OperandType.IndexedAddress;
                var idContext = context.indexedAddr();
                regContext = idContext.reg16Idx();
                if (idContext.ChildCount > 3)
                {
                    Expression = visitorContext.GetExpression(idContext.expr());
                }
                Register = idContext.reg16Idx().NormalizeToken();
                Sign = idContext.ChildCount > 3
                    ? idContext.GetChild(2).NormalizeToken()
                    : null;
                if (idContext.LPAR() != null)
                {
                    visitorContext.AddOperand(idContext.LPAR());
                }
                if (idContext.RPAR() != null)
                {
                    visitorContext.AddOperand(idContext.RPAR());
                }
            }
            else if (context.expr() != null)
            {
                Type = OperandType.Expr;
                Expression = visitorContext.GetExpression(context.expr());
            }
            else if (context.condition() != null)
            {
                Type = OperandType.Condition;
                Condition = context.condition().NormalizeToken();
                regContext = context.condition();
            }
            else if (context.macroParam() != null)
            {
                // --- LREG or HREG with macro parameter
                visitorContext.AddFunction(context);
                visitorContext.AddMacroParam(context.macroParam());
                if (context.macroParam().IDENTIFIER() != null)
                {
                    visitorContext.AddMacroParamName(context.macroParam().IDENTIFIER().NormalizeToken());
                }
            }
            else if (context.reg16Std() != null)
            {
                // --- LREG or HREG with 16-bit register
                visitorContext.AddFunction(context);
                Type = OperandType.Reg8;
                Register = string.Empty;

                if (context.HREG() != null)
                {
                    regContext = context.reg16Std();
                    switch (context.reg16Std().NormalizeToken())
                    {
                        case "BC":
                            Register = "B";
                            break;
                        case "DE":
                            Register = "D";
                            break;
                        case "HL":
                            Register = "H";
                            break;
                        case "IX":
                            Register = "IXH";
                            Type = OperandType.Reg8Idx;
                            break;
                        case "IY":
                            Register = "IYH";
                            Type = OperandType.Reg8Idx;
                            break;
                        default:
                            regContext = null;
                            break;
                    }
                }
                else
                {
                    regContext = context.reg16Std();
                    switch (context.reg16Std().NormalizeToken())
                    {
                        case "BC":
                            Register = "C";
                            break;
                        case "DE":
                            Register = "E";
                            break;
                        case "HL":
                            Register = "L";
                            break;
                        case "IX":
                            Register = "IXL";
                            Type = OperandType.Reg8Idx;
                            break;
                        case "IY":
                            Register = "IYL";
                            Type = OperandType.Reg8Idx;
                            break;
                        default:
                            regContext = null;
                            break;
                    }
                }
            }
            else if (context.NONEARG() != null)
            {
                // --- This can happen only as the result of a macro substitution
                Type = OperandType.None;
            }

            if (regContext != null)
            {
                visitorContext.AddOperand(regContext);
            }
        }
    }
}