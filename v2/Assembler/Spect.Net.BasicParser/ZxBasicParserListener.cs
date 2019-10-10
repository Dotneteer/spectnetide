using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Spect.Net.BasicParser.Generated;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Spect.Net.BasicParser
{
    /// <summary>
    /// Represents a listener that processes the ZX BASIC grammar
    /// </summary>
    public class ZxBasicParserListener: ZxBasicBaseListener
    {
        /// <summary>
        /// The map of tokens for each source code line
        /// </summary>
        public readonly Dictionary<int, List<TokenInfo>> TokenMap
            = new Dictionary<int, List<TokenInfo>>();

        /// <summary>
        /// Handle ZX BASIC keywords
        /// </summary>
        public override void ExitZxb_keyword([NotNull] ZxBasicParser.Zxb_keywordContext context)
        {
            AddSpan(TokenType.ZxbKeyword, context.Start, context.Start.Text.Length);
        }

        /// <summary>
        /// Handle ZX BASIC block comments
        /// </summary>
        public override void ExitZxb_block_comment([NotNull] ZxBasicParser.Zxb_block_commentContext context)
        {
            var text = context.GetText();
            var parts = Regex.Split(text, "\r\n");
            var startLine = context.Start.Line;
            var offset = context.Start.Column;
            for (var i = 0; i < parts.Length; i++)
            {
                AddSpan(TokenType.ZxbComment, startLine + i, offset, parts[i].Length);
                offset = 0;
            }
        }

        /// <summary>
        /// Handle ZXB line comments
        /// </summary>
        public override void ExitZxb_line_comment([NotNull] ZxBasicParser.Zxb_line_commentContext context)
        {
            var text = context.GetText();            
            if (text.ToUpper().StartsWith("REM"))
            {
                AddSpan(TokenType.ZxbKeyword, context.Start, 3);
                AddSpan(TokenType.ZxbComment, context.Start.Line, context.Start.Column + 3, context.GetText().Length - 3);
            }
            else
            {
                AddSpan(TokenType.ZxbComment, context.Start, context.GetText().Length);
            }
        }

        /// <summary>
        /// Handle ZX BASIC function tokens
        /// </summary>
        public override void ExitZxb_function([NotNull] ZxBasicParser.Zxb_functionContext context)
        {
            AddSpan(TokenType.ZxbFunction, context.Start, context.Start.Text.Length);
        }

        /// <summary>
        /// Handle ZX BASIC operator tokens
        /// </summary>
        public override void ExitZxb_operator([NotNull] ZxBasicParser.Zxb_operatorContext context)
        {
            AddSpan(TokenType.ZxbOperator, context.Start, context.Start.Text.Length);
        }

        /// <summary>
        /// Handle ZX BASIC identifier tokens
        /// </summary>
        public override void ExitZxb_identifier([NotNull] ZxBasicParser.Zxb_identifierContext context)
        {
            AddSpan(TokenType.ZxbIdentifier, context.Start, context.Start.Text.Length);
        }

        /// <summary>
        /// Handle ZX BASIC number tokens
        /// </summary>
        public override void ExitZxb_number([NotNull] ZxBasicParser.Zxb_numberContext context)
        {
            AddSpan(TokenType.ZxbNumber, context.Start, context.Start.Text.Length);
        }

        /// <summary>
        /// Handle ZX BASIC string tokens
        /// </summary>
        public override void ExitZxb_string([NotNull] ZxBasicParser.Zxb_stringContext context)
        {
            AddSpan(TokenType.ZxbString, context.Start, context.Start.Text.Length);
        }

        public override void ExitZxb_label([NotNull] ZxBasicParser.Zxb_labelContext context)
        {
            AddSpan(TokenType.ZxbLabel, context.Start, context.GetText().Length);
        }

        public override void ExitZxb_asm_start([NotNull] ZxBasicParser.Zxb_asm_startContext context)
        {
            AddSpan(TokenType.ZxbAsm, context.Start, context.GetText().Length);
        }

        public override void ExitZxb_asm_end([NotNull] ZxBasicParser.Zxb_asm_endContext context)
        {
            AddSpan(TokenType.ZxbAsm, context.Start, context.Start.Text.Length);
            AddSpan(TokenType.ZxbAsm, context.Stop, context.Stop.Text.Length);
        }

        /// <summary>
        /// Adds a span to the token map
        /// </summary>
        /// <param name="type">Token type</param>
        /// <param name="startToken">Start token</param>
        /// <param name="length">Span length</param>
        private void AddSpan(TokenType type, IToken startToken, int length)
        {
            if (startToken == null) return;
            var span = new TokenInfo(type, startToken.Column, length);
            AddSpan(startToken.Line, span);
        }

        /// <summary>
        /// Add a span to the token map
        /// </summary>
        /// <param name="type"></param>
        /// <param name="line"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void AddSpan(TokenType type, int line, int start, int length)
        {
            AddSpan(line, new TokenInfo(type, start, length));
        }

        /// <summary>
        /// Adds a span to the token map
        /// </summary>
        /// <param name="type">Token type</param>
        /// <param name="line">Line to add this token for</param>
        /// <param name="tokenInfo">Token information to add</param>
        private void AddSpan(int line, TokenInfo tokenInfo)
        {
            if (TokenMap.TryGetValue(line, out var tokenList))
            {
                tokenList.Add(tokenInfo);
            }
            else
            {
                TokenMap.Add(line, new List<TokenInfo> { tokenInfo });
            }
        }
    }
}
