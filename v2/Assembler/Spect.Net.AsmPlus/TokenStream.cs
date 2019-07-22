using System;
using System.Collections.Generic;

namespace Spect.Net.AsmPlus
{
    /// <summary>
    /// This class implements the lexer that can get the next token from the
    /// Z80 Assembly source code
    /// </summary>
    public class TokenStream
    {
        private readonly string _src;
        private readonly int _srcLen;
        private int _pos;

        /// <summary>
        /// Initializes the lexer with the specified source code
        /// </summary>
        /// <param name="source">Input source code</param>
        public TokenStream(string source)
        {
            _src = source ?? throw new ArgumentNullException(nameof(source));
            _srcLen = source.Length;
            _pos = 0;
        }

        /// <summary>
        /// Gets the next token from the stream
        /// </summary>
        /// <returns></returns>
        public TokenType GetNext(out string token)
        {
            var startPos = _pos;
            var endFound = false;
            var phase = Phase.Null;
            var tokenOnEof = TokenType.Error;
            var keywordLike = false;
            while (!endFound)
            {
                // --- No more character
                if (_pos >= _srcLen)
                {
                    break;
                }
                var ch = _src[_pos++];
                switch (phase)
                {
                    // --- Start reading a new token
                    case Phase.Null:
                        // --- Check for explicit start
                        switch (ch)
                        {
                            case ':':
                                phase = Phase.Colon;
                                tokenOnEof = TokenType.Colon;
                                break;
                            case ';':
                                phase = Phase.EolCommentWaitForEnd;
                                tokenOnEof = TokenType.Comment;
                                break;
                            case '/':
                                phase = Phase.Slash;
                                tokenOnEof = TokenType.DivOp;
                                break;
                            case ',':
                                token = ",";
                                return TokenType.Comma;
                            case '=':
                                tokenOnEof = TokenType.Assign;
                                phase = Phase.Equal1;
                                break;
                            case '(':
                                token = "(";
                                return TokenType.Lpar;
                            case ')':
                                token = ")";
                                return TokenType.Rpar;
                            case '[':
                                token = "[";
                                return TokenType.Lsbrac;
                            case ']':
                                token = "]";
                                return TokenType.Rsbrac;
                            case '?':
                                token = "?";
                                return TokenType.Qmark;
                            case '+':
                                token = "+";
                                return TokenType.Plus;
                            case '-':
                                tokenOnEof = TokenType.Minus;
                                phase = Phase.GoesTo;
                                break;
                            case '|':
                                token = "|";
                                return TokenType.VBar;
                            case '^':
                                token = "^";
                                return TokenType.UpArr;
                            case '&':
                                token = "&";
                                return TokenType.Amp;
                            case '!':
                                tokenOnEof = TokenType.Exclm;
                                phase = Phase.NEqual1;
                                break;
                            case '<':
                                phase = Phase.LessThan;
                                tokenOnEof = TokenType.LtOp;
                                break;
                            case '>':
                                phase = Phase.GreaterThan;
                                tokenOnEof = TokenType.GtOp;
                                break;
                            case '*':
                                token = "*";
                                return TokenType.MulOp;
                            case '~':
                                token = "~";
                                return TokenType.Tilde;
                            case '%':
                                phase = Phase.Modulo;
                                tokenOnEof = TokenType.ModOp;
                                break;
                            case '{':
                                phase = Phase.LBrace;
                                break;
                            case '}':
                                phase = Phase.RBrace;
                                break;
                            case '.':
                                phase = Phase.Dot;
                                tokenOnEof = TokenType.Dot;
                                break;
                            case '#':
                                phase = Phase.PreProcLike;
                                break;
                            case '$':
                                phase = Phase.Dollar;
                                tokenOnEof = TokenType.CurAddr;
                                break;
                            case '\'':
                            case '\"':
                                phase = Phase.CollectCharacter;
                                break;
                        }

                        if (phase != Phase.Null)
                        {
                            // --- We have a next phase
                            break;
                        }

                        // --- Test for identifier start
                        if (ch == '_' || ch == '@' || ch == '`' || ch >= 'A' && ch <= 'Z'
                            || ch >= 'a' && ch <= 'z')
                        {
                            phase = Phase.IdLike;
                            keywordLike = true;
                            tokenOnEof = TokenType.Identifier;
                            break;
                        }

                        // --- Test for a potential numeric literal
                        if (char.IsDigit(ch))
                        {
                            phase = Phase.NumberLike;
                            tokenOnEof = TokenType.DecNum;
                            break;
                        }

                        endFound = true;
                        break;

                    // --- We received a colon. Continuations: ':', '=', ''
                    case Phase.Colon:
                        switch (ch)
                        {
                            case ':':
                                token = "::";
                                return TokenType.Dcolon;
                            case '=':
                                token = ":=";
                                return TokenType.VarPrag;
                            default:
                                _pos--;
                                token = ":";
                                return TokenType.Colon;
                        }

                    // --- We received a '/'. Continuations: '/', '*', ''
                    case Phase.Slash:
                        switch (ch)
                        {
                            case '/':
                                phase = Phase.EolCommentWaitForEnd;
                                break;
                            case '*':
                                phase = Phase.BlockCommentWaitForEnd1;
                                break;
                            default:
                                _pos--;
                                token = GetToken();
                                return TokenType.DivOp;
                        }
                        break;

                    // --- Check if the EOL comment is complete
                    case Phase.EolCommentWaitForEnd:
                        if (ch == '\r' || ch == '\n')
                        {
                            _pos--;
                            token = GetToken();
                            return TokenType.Comment;
                        }
                        break;

                    // --- Process the block comment, end check for the '*' terminator start 
                    case Phase.BlockCommentWaitForEnd1:
                        if (ch == '\r' || ch == '\n')
                        {
                            // --- A block comment cannot contain line break
                            _pos--;
                            token = GetToken();
                            return TokenType.Error;
                        }

                        if (ch == '*')
                        {
                            phase = Phase.BlockCommentWaitForEnd2;
                        }
                        break;

                    // --- Process the block comment, end check for the '/' terminator start 
                    case Phase.BlockCommentWaitForEnd2:
                        switch (ch)
                        {
                            case '\r':
                            case '\n':
                                // --- A block comment cannot contain line break
                                _pos--;
                                token = GetToken();
                                return TokenType.Error;
                            case '/':
                                token = GetToken();
                                return TokenType.Comment;
                            case '*':
                                break;
                            default:
                                phase = Phase.BlockCommentWaitForEnd1;
                                break;
                        }
                        break;

                    // --- Check for a second '=' character after "="
                    case Phase.Equal1:
                        if (ch != '=')
                        {
                            _pos--;
                            token = "=";
                            return TokenType.Assign;
                        }
                        phase = Phase.Equal2;
                        tokenOnEof = TokenType.EqOp;
                        break;

                    // --- Check for a third '=' character after "=="
                    case Phase.Equal2:
                        if (ch == '=')
                        {
                            token = "===";
                            return TokenType.CiEqOp;
                        }
                        _pos--;
                        token = "==";
                        return TokenType.EqOp;

                    case Phase.GoesTo:
                        if (ch == '>')
                        {
                            token = "->";
                            return TokenType.GoesTo;
                        }
                        _pos--;
                        token = "-";
                        return TokenType.Minus;

                    // --- Check for a second '=' character after "!"
                    case Phase.NEqual1:
                        if (ch != '=')
                        {
                            _pos--;
                            token = "!";
                            return TokenType.Exclm;
                        }
                        phase = Phase.NEqual2;
                        tokenOnEof = TokenType.NeqOp;
                        break;

                    // --- Check for a third '=' character after "!="
                    case Phase.NEqual2:
                        if (ch == '=')
                        {
                            token = "!==";
                            return TokenType.CiNeqOp;
                        }
                        _pos--;
                        token = "!=";
                        return TokenType.NeqOp;

                    // --- We received a '<'. Continuations: '=', '<', '?', (fstring), ''
                    case Phase.LessThan:
                        switch (ch)
                        {
                            case '=':
                                token = "<=";
                                return TokenType.LteOp;
                            case '<':
                                token = "<<";
                                return TokenType.LshOp;
                            case '?':
                                token = "<?";
                                return TokenType.MinOp;
                            // TODO: Handle the FSTRING case
                            default:
                                _pos--;
                                token = "<";
                                return TokenType.LtOp;
                        }

                    // --- We received a '>'. Continuations: '=', '>', '?', ''
                    case Phase.GreaterThan:
                        switch (ch)
                        {
                            case '=':
                                token = ">=";
                                return TokenType.GteOp;
                            case '>':
                                token = ">>";
                                return TokenType.RshOp;
                            case '?':
                                token = ">?";
                                return TokenType.MaxOp;
                            default:
                                _pos--;
                                token = ">";
                                return TokenType.GtOp;
                        }

                    // --- Check for next character after '%'
                    case Phase.Modulo:
                        if (ch == '0' || ch == '1')
                        {
                            phase = Phase.BinaryWithModPrefix;
                            break;
                        }
                        _pos--;
                        token = "%";
                        return TokenType.ModOp;

                    // --- Check for next character after '{'
                    case Phase.LBrace:
                        if (ch == '{')
                        {
                            token = "{{";
                            return TokenType.LdBrac;
                        }
                        _pos--;
                        endFound = true;
                        break;

                    // --- Check for next character after '}'
                    case Phase.RBrace:
                        if (ch == '}')
                        {
                            token = "}}";
                            return TokenType.RdBrac;
                        }
                        _pos--;
                        endFound = true;
                        break;

                    // --- Check for next character after '.'
                    case Phase.Dot:
                        if (char.IsLetter(ch))
                        {
                            phase = Phase.KeywordLike;
                            keywordLike = true;
                            break;
                        }
                        _pos--;
                        token = ".";
                        return TokenType.Dot;

                    // --- Check for next character after '.'
                    case Phase.Dollar:
                        // TODO: Implement this branch
                        endFound = true;
                        break;

                    // --- Get binary digits after '%'
                    case Phase.BinaryWithModPrefix:
                        // TODO: Implement this branch
                        endFound = true;
                        break;

                    // --- Get keyword-like construct
                    case Phase.KeywordLike:
                        if (!char.IsLetter(ch))
                        {
                            _pos--;
                            keywordLike = true;
                            endFound = true;
                        }
                        break;

                    // --- Get ID-like construct
                    case Phase.IdLike:
                        if (ch == '_' || ch == '@' || ch == '`' 
                            || ch == '!' || ch == '?' || ch == '#'
                            || ch >= 'A' && ch <= 'Z'
                            || ch >= 'a' && ch <= 'z')
                        {
                            phase = Phase.IdLike;
                            tokenOnEof = TokenType.Identifier;
                            break;
                        }
                        if (ch != '\'')
                        {
                            // --- This must be the last character of and ID (AF')
                            _pos--;
                        }
                        endFound = true;
                        break;

                    // --- Get preproc-like construct
                    case Phase.PreProcLike:
                        // TODO: Implement this branch
                        endFound = true;
                        break;

                    // --- Get ID-like construct
                    case Phase.CollectCharacter:
                        // TODO: Implement this branch
                        endFound = true;
                        break;

                    // --- No its the end, probably an error
                    default:
                        endFound = true;
                        break;
                }
            }

            token = GetToken();
            return PostProcess(token);

            string GetToken()
            {
                var len = (_pos > _srcLen - 1 ? _srcLen : _pos) - startPos;
                return _src.Substring(startPos, len >= 0 ? len : 0);
            }

            TokenType PostProcess(string tokenText)
            {
                if (phase == Phase.Null) return TokenType.Eof;
                if (!keywordLike) return tokenOnEof;

                if (s_Mnemonics.TryGetValue(tokenText, out var mnemonic))
                {
                    return mnemonic;
                }

                if (s_Regs.TryGetValue(tokenText, out var reg))
                {
                    return reg;
                }

                if (s_Statements.TryGetValue(tokenText, out var stmt))
                {
                    return stmt;
                }
                return TokenType.Error;
            }
        }

        /// <summary>
        /// Represents the current parsing phase
        /// </summary>
        private enum Phase
        {
            // --- Right at the beginning of a new token
            Null,

            // --- Check the next char after colon
            Colon,

            // --- Check the content after slash
            Slash,

            // --- Look for the end of the EOL comment
            EolCommentWaitForEnd,

            // --- Look for the end of the block comment ('*')
            BlockCommentWaitForEnd1,

            // --- Look for the end of the block comment ('/')
            BlockCommentWaitForEnd2,

            // --- Look for a second '=' character after "="
            Equal1,

            // --- Look for a third '=' character after "=="
            Equal2,

            // --- Look for a GoesTo ending, '>'
            GoesTo,

            // --- Look for a second '=' character after "!"
            NEqual1,

            // --- Look for a third '=' character after "!="
            NEqual2,

            // --- Check the next character after '<'
            LessThan,

            // --- Check the next character after '>'
            GreaterThan,

            // --- Check the next character after '%'
            Modulo,

            // --- Getting the details of a binary number after '%'
            BinaryWithModPrefix,

            // --- Check next character after '{'
            LBrace,

            // --- Check next character after '}'
            RBrace,

            // --- Check the character after '.'
            Dot,

            // --- Obtain a keyword-like construct
            KeywordLike,

            // --- Obtain an ID-like construct
            IdLike,

            // --- Obtain a preproc-like construct
            PreProcLike,

            // --- Obtain a number-like construct
            NumberLike,

            // --- Obtaining a token starting with '$'
            Dollar,

            // --- Collect a character
            CollectCharacter,
        }

        // --- Mnemonics and their tokens
        private static readonly Dictionary<string, TokenType> s_Mnemonics = new Dictionary<string, TokenType>
        {
            {"nop", TokenType.Nop},
            {"NOP", TokenType.Nop},
            {"rlca", TokenType.Rlca},
            {"RLCA", TokenType.Rlca},
            {"rrca", TokenType.Rrca},
            {"RRCA", TokenType.Rrca},
            {"rla", TokenType.Rla},
            {"RLA", TokenType.Rla},
            {"rra", TokenType.Rra},
            {"RRA", TokenType.Rra},
            {"daa", TokenType.Daa},
            {"DAA", TokenType.Daa},
            {"cpl", TokenType.Cpl},
            {"CPL", TokenType.Cpl},
            {"scf", TokenType.Scf},
            {"SCF", TokenType.Scf},
            {"ccf", TokenType.Ccf},
            {"CCF", TokenType.Ccf},
            {"halt", TokenType.Halt},
            {"HALT", TokenType.Halt},
            {"exx", TokenType.Exx},
            {"EXX", TokenType.Exx},
            {"di", TokenType.Di},
            {"DI", TokenType.Di},
            {"ei", TokenType.Ei},
            {"EI", TokenType.Ei},
            {"neg", TokenType.Neg},
            {"NEG", TokenType.Neg},
            {"retn", TokenType.Retn},
            {"RETN", TokenType.Retn},
            {"reti", TokenType.Reti},
            {"RETI", TokenType.Reti},
            {"rld", TokenType.Rld},
            {"RLD", TokenType.Rld},
            {"rrd", TokenType.Rrd},
            {"RRD", TokenType.Rrd},
            {"ldi", TokenType.Ldi},
            {"LDI", TokenType.Ldi},
            {"cpi", TokenType.Cpi},
            {"CPI", TokenType.Cpi},
            {"ini", TokenType.Ini},
            {"INI", TokenType.Ini},
            {"outi", TokenType.Outi},
            {"OUTI", TokenType.Outi},
            {"ldd", TokenType.Ldd},
            {"LDD", TokenType.Ldd},
            {"cpd", TokenType.Cpd},
            {"CPD", TokenType.Cpd},
            {"ind", TokenType.Ind},
            {"IND", TokenType.Ind},
            {"outd", TokenType.Outd},
            {"OUTD", TokenType.Outd},
            {"ldir", TokenType.Ldir},
            {"LDIR", TokenType.Ldir},
            {"cpir", TokenType.Cpir},
            {"CPIR", TokenType.Cpir},
            {"inir", TokenType.Inir},
            {"INIR", TokenType.Inir},
            {"otir", TokenType.Otir},
            {"OTIR", TokenType.Otir},
            {"lddr", TokenType.Lddr},
            {"LDDR", TokenType.Lddr},
            {"cpdr", TokenType.Cpdr},
            {"CPDR", TokenType.Cpdr},
            {"indr", TokenType.Indr},
            {"INDR", TokenType.Indr},
            {"otdr", TokenType.Otdr},
            {"OTDR", TokenType.Otdr},
            { "ld", TokenType.Ld},
            { "LD", TokenType.Ld},
            { "inc", TokenType.Inc},
            { "INC", TokenType.Inc},
            { "dec", TokenType.Dec},
            { "DEC", TokenType.Dec},
            { "ex", TokenType.Ex},
            { "EX", TokenType.Ex},
            { "add", TokenType.Add},
            { "ADD", TokenType.Add},
            { "adc", TokenType.Adc},
            { "ADC", TokenType.Adc},
            { "sub", TokenType.Sub},
            { "SUB", TokenType.Sub},
            { "sbc", TokenType.Sbc},
            { "SBC", TokenType.Sbc},
            { "and", TokenType.And},
            { "AND", TokenType.And},
            { "xor", TokenType.Xor},
            { "XOR", TokenType.Xor},
            { "or", TokenType.Or},
            { "OR", TokenType.Or},
            { "cp", TokenType.Cp},
            { "CP", TokenType.Cp},
            { "djnz", TokenType.Djnz},
            { "DJNZ", TokenType.Djnz},
            { "jr", TokenType.Jr},
            { "JR", TokenType.Jr},
            { "jp", TokenType.Jp},
            { "JP", TokenType.Jp},
            { "call", TokenType.Call},
            { "CALL", TokenType.Call},
            { "ret", TokenType.Ret},
            { "RET", TokenType.Ret},
            { "rst", TokenType.Rst},
            { "RST", TokenType.Rst},
            { "push", TokenType.Push},
            { "PUSH", TokenType.Push},
            { "pop", TokenType.Pop},
            { "POP", TokenType.Pop},
            { "in", TokenType.In},
            { "IN", TokenType.In},
            { "out", TokenType.Out},
            { "OUT", TokenType.Out},
            { "im", TokenType.Im},
            { "IM", TokenType.Im},
            { "rlc", TokenType.Rlc},
            { "RLC", TokenType.Rlc},
            { "rrc", TokenType.Rrc},
            { "RRC", TokenType.Rrc},
            { "rl", TokenType.Rl},
            { "RL", TokenType.Rl},
            { "rr", TokenType.Rr},
            { "RR", TokenType.Rr},
            { "sla", TokenType.Sla},
            { "SLA", TokenType.Sla},
            { "sra", TokenType.Sra},
            { "SRA", TokenType.Sra},
            { "sll", TokenType.Sll},
            { "SLL", TokenType.Sll},
            { "srl", TokenType.Srl},
            { "SRL", TokenType.Srl},
            { "bit", TokenType.Bit},
            { "BIT", TokenType.Bit},
            { "res", TokenType.Res},
            { "RES", TokenType.Res},
            { "set", TokenType.Set},
            { "SET", TokenType.Set},
            { "mirror", TokenType.Mirror},
            { "MIRROR", TokenType.Mirror},
            { "test", TokenType.Test},
            { "TEST", TokenType.Test},
            { "nextreg", TokenType.NextReg},
            { "NEXTREG", TokenType.NextReg},
            { "swapnib", TokenType.SwapNib},
            { "SWAPNIB", TokenType.SwapNib},
            { "mul", TokenType.Mul},
            { "MUL", TokenType.Mul},
            { "outinb", TokenType.Outinb},
            { "OUTINB", TokenType.Outinb},
            { "ldix", TokenType.Ldix},
            { "LDIX", TokenType.Ldix},
            { "ldirx", TokenType.Ldirx},
            { "LDIRX", TokenType.Ldirx},
            { "lddx", TokenType.Lddx},
            { "LDDX", TokenType.Lddx},
            { "lddrx", TokenType.Lddrx},
            { "LDDRX", TokenType.Lddrx},
            { "pixeldn", TokenType.Pixeldn},
            { "PIXELDN", TokenType.Pixeldn},
            { "pixelad", TokenType.Pixelad},
            { "PIXELAD", TokenType.Pixelad},
            { "setae", TokenType.Setae},
            { "SETAE", TokenType.Setae},
            { "ldpirx", TokenType.Ldpirx},
            { "LDPIRX", TokenType.Ldpirx},
            { "ldirscale", TokenType.Ldirscale},
            { "LDIRSCALE", TokenType.Ldirscale},
        };

        // --- Registers, flags, conditions, and their tokens
        private static readonly Dictionary<string, TokenType> s_Regs = new Dictionary<string, TokenType>
        {
            { "a", TokenType.A },
            { "A", TokenType.A },
            { "b", TokenType.B },
            { "B", TokenType.B },
            { "c", TokenType.C },
            { "C", TokenType.C },
            { "d", TokenType.D },
            { "D", TokenType.D },
            { "e", TokenType.E },
            { "E", TokenType.E },
            { "h", TokenType.H },
            { "H", TokenType.H },
            { "l", TokenType.L },
            { "L", TokenType.L },
            { "i", TokenType.I },
            { "I", TokenType.I },
            { "r", TokenType.R },
            { "R", TokenType.R },
            { "xl", TokenType.Xl },
            { "XL", TokenType.Xl },
            { "xh", TokenType.Xh },
            { "XH", TokenType.Xh },
            { "yl", TokenType.Yl },
            { "YL", TokenType.Yl },
            { "yh", TokenType.Yh },
            { "YH", TokenType.Yh },
            { "ixl", TokenType.Xl },
            { "IXL", TokenType.Xl },
            { "IXl", TokenType.Xl },
            { "ixh", TokenType.Xh },
            { "IXH", TokenType.Xh },
            { "IXh", TokenType.Xh },
            { "iyl", TokenType.Yl },
            { "IYL", TokenType.Yl },
            { "IYl", TokenType.Yl },
            { "iyh", TokenType.Yh },
            { "IYH", TokenType.Yh },
            { "IYh", TokenType.Yh },
            { "bc", TokenType.Bc },
            { "BC", TokenType.Bc },
            { "de", TokenType.De },
            { "DE", TokenType.De },
            { "hl", TokenType.Hl },
            { "HL", TokenType.Hl },
            { "sp", TokenType.Sp },
            { "SP", TokenType.Sp },
            { "ix", TokenType.Ix },
            { "IX", TokenType.Ix },
            { "iy", TokenType.Iy },
            { "IY", TokenType.Iy },
            { "af", TokenType.Af },
            { "AF", TokenType.Af },
            { "af'", TokenType.Af_ },
            { "AF'", TokenType.Af_ },
            { "z", TokenType.Z },
            { "Z", TokenType.Z },
            { "nz", TokenType.Nz },
            { "NZ", TokenType.Nz },
            { "nc", TokenType.Nc },
            { "NC", TokenType.Nc },
            { "po", TokenType.Po },
            { "PO", TokenType.Po },
            { "pe", TokenType.Pe },
            { "PE", TokenType.Pe },
            { "p", TokenType.P },
            { "P", TokenType.P },
            { "m", TokenType.M },
            { "M", TokenType.M },
        };

        // --- Statements and their tokens
        private static readonly Dictionary<string, TokenType> s_Statements = new Dictionary<string, TokenType>
        {
            { "macro", TokenType.Macro },
            { "MACRO", TokenType.Macro },
            { ".macro", TokenType.Macro },
            { ".MACRO", TokenType.Macro },
            { "endm", TokenType.EndMacro },
            { "ENDM", TokenType.EndMacro },
            { ".endm", TokenType.EndMacro },
            { ".ENDM", TokenType.EndMacro },
            { ".proc", TokenType.Proc },
            { ".PROC", TokenType.Proc },
            { ".endp", TokenType.EndProc },
            { ".ENDP", TokenType.EndProc },
            { ".pend", TokenType.EndProc },
            { ".PEND", TokenType.EndProc },
            { ".loop", TokenType.Loop },
            { ".LOOP", TokenType.Loop },
            { ".endl", TokenType.EndLoop },
            { ".ENDL", TokenType.EndLoop },
            { ".lend", TokenType.EndLoop },
            { ".LEND", TokenType.EndLoop },
            { ".repeat", TokenType.Repeat },
            { ".REPEAT", TokenType.Repeat },
            { ".until", TokenType.Until },
            { ".UNTIL", TokenType.Until },
            { ".while", TokenType.While },
            { ".WHILE", TokenType.While },
            { ".endw", TokenType.EndWhile },
            { ".ENDW", TokenType.EndWhile },
            { ".wend", TokenType.EndWhile },
            { ".WEND", TokenType.EndWhile },
            { "if", TokenType.IfStmt },
            { "IF", TokenType.IfStmt },
            { ".if", TokenType.IfStmt },
            { ".IF", TokenType.IfStmt },
            { ".ifused", TokenType.IfUsed },
            { ".IFUSED", TokenType.IfUsed },
            { ".ifnused", TokenType.IfNused },
            { ".IFNUSED", TokenType.IfNused },
            { ".elif", TokenType.Elif },
            { ".ELIF", TokenType.Elif },
            { ".else", TokenType.ElseStmt },
            { ".ELSE", TokenType.ElseStmt },
            { ".endif", TokenType.EndIfStmt },
            { ".ENDIF", TokenType.EndIfStmt },
            { ".for", TokenType.For },
            { ".FOR", TokenType.For },
            { "for", TokenType.For },
            { "FOR", TokenType.For },
            { ".to", TokenType.To },
            { ".TO", TokenType.To },
            { "to", TokenType.To },
            { "TO", TokenType.To },
            { ".step", TokenType.Step },
            { ".STEP", TokenType.Step },
            { "step", TokenType.Step },
            { "STEP", TokenType.Step },
            { ".next", TokenType.ForNext },
            { ".NEXT", TokenType.ForNext },
            { "next", TokenType.ForNext },
            { "NEXT", TokenType.ForNext },
            { ".break", TokenType.Break },
            { ".BREAK", TokenType.Break },
            { "break", TokenType.Break },
            { "BREAK", TokenType.Break },
            { ".continue", TokenType.Continue },
            { ".CONTINUE", TokenType.Continue },
            { "continue", TokenType.Continue },
            { "CONTINUE", TokenType.Continue },
        };
    }
}