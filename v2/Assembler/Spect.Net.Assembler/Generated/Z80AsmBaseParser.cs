using System.Linq;
using Antlr4.Runtime;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace Spect.Net.Assembler.Generated
{
    /// <summary>
    /// Contains parser instructions
    /// </summary>
    public abstract class Z80AsmBaseParser: Parser
    {
        protected Z80AsmBaseParser(ITokenStream input) : base(input)
        {
        }

        /// <summary>
        /// Short form for prev(String str)
        /// </summary>
        protected bool p(params string[] str)
        {
            return prev(str);
        }

        /// <summary>
        /// Whether the previous token value equals to str
        /// </summary>
        protected bool prev(params string[] str)
        {
            return str.Any(s => _input.Lt(-1).Text.Equals(s) || _input.Lt(-1).Text.Equals(s.ToUpper()));
        }
        
        /// <summary>
        /// Short form for next(String str)
        /// </summary>
        protected bool n(params string[] str)
        {
            return next(str);
        }

        // 
        /// <summary>
        /// Tests whether the next token value equals to @param str 
        /// </summary>
        /// <param name="str">String to check</param>
        protected bool next(params string[] str)
        {
            return str.Any(s => _input.Lt(1).Text.Equals(s));
        }

        protected bool exprStart()
        {
            return ExpressionStartToken.Any(token => _input.Lt(1).Type == token);
        }

        protected static int[] ExpressionStartToken =
        {
            // --- builtInFunctionInvocation
            Z80AsmLexer.TEXTOF,
            Z80AsmLexer.LTEXTOF,
            Z80AsmLexer.DEF,
            Z80AsmLexer.ISREG8,
            Z80AsmLexer.ISREG8STD,
            Z80AsmLexer.ISREG8SPEC,
            Z80AsmLexer.ISREG8IDX,
            Z80AsmLexer.ISREG16,
            Z80AsmLexer.ISREG16STD,
            Z80AsmLexer.ISREG16IDX,
            Z80AsmLexer.ISREGINDIRECT,
            Z80AsmLexer.ISCPORT,
            Z80AsmLexer.ISINDEXEDADDR,
            Z80AsmLexer.ISCONDITION,
            Z80AsmLexer.ISEXPR,
            Z80AsmLexer.ISREGA,
            Z80AsmLexer.ISREGAF,
            Z80AsmLexer.ISREGB,
            Z80AsmLexer.ISREGC,
            Z80AsmLexer.ISREGBC,
            Z80AsmLexer.ISREGD,
            Z80AsmLexer.ISREGE,
            Z80AsmLexer.ISREGDE,
            Z80AsmLexer.ISREGH,
            Z80AsmLexer.ISREGL,
            Z80AsmLexer.ISREGHL,
            Z80AsmLexer.ISREGI,
            Z80AsmLexer.ISREGR,
            Z80AsmLexer.ISREGXH,
            Z80AsmLexer.ISREGXL,
            Z80AsmLexer.ISREGIX,
            Z80AsmLexer.ISREGYH,
            Z80AsmLexer.ISREGYL,
            Z80AsmLexer.ISREGIY,
            Z80AsmLexer.ISREGSP,

            // --- functionInvocation
            Z80AsmLexer.IDENTIFIER,

            // --- macroParam
            Z80AsmLexer.LDBRAC,

            // --- Unary expressions
            Z80AsmLexer.PLUS,
            Z80AsmLexer.MINUS,
            Z80AsmLexer.TILDE,
            Z80AsmLexer.EXCLM,

            // --- Parenthesized expressions
            Z80AsmLexer.LSBRAC,
            Z80AsmLexer.LPAR,

            // --- Literals
            Z80AsmLexer.HEXNUM,
            Z80AsmLexer.DECNUM,
            Z80AsmLexer.OCTNUM,
            Z80AsmLexer.CHAR,
            Z80AsmLexer.BINNUM,
            Z80AsmLexer.REALNUM,
            Z80AsmLexer.BOOLLIT,
            Z80AsmLexer.STRING,
            Z80AsmLexer.CURADDR,
            Z80AsmLexer.DOT,
            Z80AsmLexer.MULOP,
            Z80AsmLexer.CURCNT,
            Z80AsmLexer.DCOLON
        };
    }
}