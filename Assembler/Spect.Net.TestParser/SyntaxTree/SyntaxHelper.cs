using System.Collections.Generic;
using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class contains helper methods for creating the syntax tree
    /// </summary>
    public static class SyntaxHelper
    {
        /// <summary>
        /// Normalizes a token by omitting whitespaces and converting to uppercase
        /// </summary>
        /// <param name="context">Context to get the element from</param>
        /// <param name="childIndex">Child index</param>
        /// <returns>Normalized token</returns>
        public static TextSpan CreateSpan(this ParserRuleContext context, int childIndex)
            => new TextSpan(context.GetChild(childIndex));

        /// <summary>
        /// Normalizes a token by omitting whitespaces and converting to uppercase
        /// </summary>
        /// <param name="context">Context to get the element from</param>
        /// <param name="childIndex">Child index</param>
        /// <returns>Normalized token</returns>
        public static string GetTokenText(this ParserRuleContext context, int childIndex)
            => context.GetChild(childIndex)?.GetText();

        /// <summary>
        /// Normalizes a token by omitting whitespaces and converting to uppercase
        /// </summary>
        /// <param name="element">Parse tree to get the element from</param>
        /// <returns>Normalized token</returns>
        public static string NormalizeToken(this IParseTree element)
            => element?.GetText().NormalizeToken();

        /// <summary>
        /// Normalizes a token by omitting whitespaces and converting to uppercase
        /// </summary>
        /// <param name="token">Token to normalize</param>
        /// <returns>Normalized token</returns>
        public static string NormalizeToken(this string token)
            => token?.ToUpperInvariant();

        /// <summary>
        /// Normalizes a string by replacing double quote escapes
        /// </summary>
        /// <param name="element">Parse tree to get the element from</param>
        /// <returns>Normalized token</returns>
        public static string NormalizeString(this IParseTree element)
            => element?.GetText().Replace("\\\"", "\"").Replace("\"", "");

        /// <summary>
        /// Converts a ZX Spectrum string into a byte lisy
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Bytes representing the string</returns>
        public static List<byte> SpectrumStringToBytes(string input)
        {
            var bytes = new List<byte>(input.Length);
            var state = StrParseState.Normal;
            var collect = 0;
            foreach (var ch in input)
            {
                switch (state)
                {
                    case StrParseState.Normal:
                        if (ch == '\\')
                        {
                            state = StrParseState.Backslash;
                        }
                        else
                        {
                            bytes.Add((byte)ch);
                        }
                        break;

                    case StrParseState.Backslash:
                        state = StrParseState.Normal;
                        switch (ch)
                        {
                            case 'i': // INK
                                bytes.Add(0x10);
                                break;
                            case 'p': // PAPER
                                bytes.Add(0x11);
                                break;
                            case 'f': // FLASH
                                bytes.Add(0x12);
                                break;
                            case 'b': // BRIGHT
                                bytes.Add(0x13);
                                break;
                            case 'I': // INVERSE
                                bytes.Add(0x14);
                                break;
                            case 'o': // OVER
                                bytes.Add(0x15);
                                break;
                            case 'a': // AT
                                bytes.Add(0x16);
                                break;
                            case 't': // TAB
                                bytes.Add(0x17);
                                break;
                            case 'P': // Pound sign
                                bytes.Add(0x60);
                                break;
                            case 'C': // Copyright sign
                                bytes.Add(0x7F);
                                break;
                            case '"':
                                bytes.Add((byte)'"');
                                break;
                            case '\'':
                                bytes.Add((byte)'\'');
                                break;
                            case '\\':
                                bytes.Add((byte)'\\');
                                break;
                            case '0':
                                bytes.Add(0);
                                break;
                            case 'x':
                                state = StrParseState.X;
                                break;
                            default:
                                bytes.Add((byte)ch);
                                break;
                        }
                        break;

                    case StrParseState.X:
                        if (ch >= '0' && ch <= '9'
                            || ch >= 'a' && ch <= 'f'
                            || ch >= 'A' && ch <= 'F')
                        {
                            collect = int.Parse(new string(ch, 1), NumberStyles.HexNumber);
                            state = StrParseState.Xh;
                        }
                        else
                        {
                            bytes.Add((byte)'x');
                            state = StrParseState.Normal;
                        }
                        break;

                    case StrParseState.Xh:
                        if (ch >= '0' && ch <= '9'
                            || ch >= 'a' && ch <= 'f'
                            || ch >= 'A' && ch <= 'F')
                        {
                            collect = collect * 0x10 + int.Parse(new string(ch, 1), NumberStyles.HexNumber);
                            bytes.Add((byte)collect);
                            state = StrParseState.Normal;
                        }
                        else
                        {
                            bytes.Add((byte)collect);
                            bytes.Add((byte)ch);
                            state = StrParseState.Normal;
                        }
                        break;
                }
            }

            // --- Handle the final machine state
            switch (state)
            {
                case StrParseState.Backslash:
                    bytes.Add((byte)'\\');
                    break;
                case StrParseState.X:
                    bytes.Add((byte)'x');
                    break;
                case StrParseState.Xh:
                    bytes.Add((byte)collect);
                    break;
            }
            return bytes;
        }

        /// <summary>
        /// We use this enumeration to represent the state
        /// of the machine parsing Spectrum string
        /// </summary>
        private enum StrParseState
        {
            Normal,
            Backslash,
            X,
            Xh
        }
    }
}