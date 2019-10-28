using System;
using System.Collections.Generic;
using Spect.Net.EvalParser.SyntaxTree;
// ReSharper disable InconsistentNaming

namespace Spect.Net.EvalParser.Test
{
    /// <summary>
    /// Expression evaluation context for unit tests
    /// </summary>
    public class Z80TestEvaluationContext: IExpressionEvaluationContext
    {
        private readonly Dictionary<string, uint> _symbols = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);

        public byte A;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;
        public byte F;
        public byte I;
        public byte R;
        public byte XL;
        public byte XH;
        public byte YL;
        public byte YH;

        public ushort AF;
        public ushort BC;
        public ushort DE;
        public ushort HL;
        public ushort AF_;
        public ushort BC_;
        public ushort DE_;
        public ushort HL_;
        public ushort SP;
        public ushort IX;
        public ushort IY;
        public ushort PC;
        public ushort WZ;

        public bool Sf;
        public bool Zf;
        public bool R5f;
        public bool Hf;
        public bool R3f;
        public bool PVf;
        public bool Nf;
        public bool Cf;

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue GetSymbolValue(string symbol)
        {
            return _symbols.TryGetValue(symbol, out var value) ? new ExpressionValue(value) : ExpressionValue.Error;
        }

        /// <summary>
        /// Gets the current value of the specified Z80 register
        /// </summary>
        /// <param name="registerName">Name of the register</param>
        /// <param name="is8Bit">Is it an 8-bit register?</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80RegisterValue(string registerName, out bool is8Bit)
        {
            is8Bit = true;
            switch (registerName.ToLower())
            {
                case "a":
                    return new ExpressionValue(A);
                case "b":
                    return new ExpressionValue(B);
                case "c":
                    return new ExpressionValue(C);
                case "d":
                    return new ExpressionValue(D);
                case "e":
                    return new ExpressionValue(E);
                case "h":
                    return new ExpressionValue(H);
                case "l":
                    return new ExpressionValue(L);
                case "f":
                    return new ExpressionValue(F);
                case "i":
                    return new ExpressionValue(I);
                case "r":
                    return new ExpressionValue(R);
                case "xh":
                case "ixh":
                    return new ExpressionValue(XH);
                case "xl":
                case "ixl":
                    return new ExpressionValue(XL);
                case "yh":
                case "iyh":
                    return new ExpressionValue(YH);
                case "yl":
                case "iyl":
                    return new ExpressionValue(YL);
                case "af":
                    is8Bit = false;
                    return new ExpressionValue(AF);
                case "bc":
                    is8Bit = false;
                    return new ExpressionValue(BC);
                case "de":
                    is8Bit = false;
                    return new ExpressionValue(DE);
                case "hl":
                    is8Bit = false;
                    return new ExpressionValue(HL);
                case "af'":
                    is8Bit = false;
                    return new ExpressionValue(AF_);
                case "bc'":
                    is8Bit = false;
                    return new ExpressionValue(BC_);
                case "de'":
                    is8Bit = false;
                    return new ExpressionValue(DE_);
                case "hl'":
                    is8Bit = false;
                    return new ExpressionValue(HL_);
                case "ix":
                    is8Bit = false;
                    return new ExpressionValue(IX);
                case "iy":
                    is8Bit = false;
                    return new ExpressionValue(IY);
                case "pc":
                    is8Bit = false;
                    return new ExpressionValue(PC);
                case "sp":
                    is8Bit = false;
                    return new ExpressionValue(SP);
                case "wz":
                    is8Bit = false;
                    return new ExpressionValue(WZ);
                default:
                    return ExpressionValue.Error;
            }
        }

        /// <summary>
        /// Gets the current value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Name of the flag</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80FlagValue(string flagName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the memory pointed by the specified Z80 register
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetMemoryIndirectValue(ExpressionValue address)
        {
            throw new System.NotImplementedException();
        }

        public void SetA(byte value)
        {
            A = value;
            AF = (ushort)((value << 8) | (AF & 0xff));
        }

        public void SetF(byte value)
        {
            F = value;
            AF = (ushort)((AF & 0xff00) | value);
            Cf = (value & 0b0000_0001) != 0;
            Nf = (value & 0b0000_0010) != 0;
            PVf = (value & 0b0000_0100) != 0;
            R3f = (value & 0b0000_1000) != 0;
            Hf = (value & 0b0001_0000) != 0;
            R5f = (value & 0b0010_0000) != 0;
            Zf = (value & 0b0100_0000) != 0;
            Sf = (value & 0b1000_0000) != 0;
        }

        public void SetB(byte value)
        {
            B = value;
            BC = (ushort)((value << 8) | (BC & 0xff));
        }

        public void SetC(byte value)
        {
            C = value;
            BC = (ushort)((BC & 0xff00) | value);
        }

        public void SetD(byte value)
        {
            D = value;
            DE = (ushort)((value << 8) | (DE & 0xff));
        }

        public void SetE(byte value)
        {
            E = value;
            DE = (ushort)((DE & 0xff00) | value);
        }

        public void SetH(byte value)
        {
            H = value;
            HL = (ushort)((value << 8) | (HL & 0xff));
        }

        public void SetL(byte value)
        {
            L = value;
            HL = (ushort)((HL & 0xff00) | value);
        }

        public void SetXH(byte value)
        {
            XH = value;
            IX = (ushort)((value << 8) | (IX & 0xff));
        }

        public void SetXL(byte value)
        {
            XL = value;
            IX = (ushort)((IX & 0xff00) | value);
        }

        public void SetYH(byte value)
        {
            YH = value;
            IY = (ushort)((value << 8) | (IY & 0xff));
        }

        public void SetYL(byte value)
        {
            YL = value;
            IY = (ushort)((IY & 0xff00) | value);
        }

        public void SetAF(ushort value)
        {
            AF = value;
            A = (byte) (value >> 8);
            F = (byte) value;
        }

        public void SetBC(ushort value)
        {
            BC = value;
            B = (byte)(value >> 8);
            C = (byte)value;
        }

        public void SetDE(ushort value)
        {
            DE = value;
            D = (byte)(value >> 8);
            E = (byte)value;
        }

        public void SetHL(ushort value)
        {
            HL = value;
            H = (byte)(value >> 8);
            L = (byte)value;
        }

        public void SetIX(ushort value)
        {
            IX = value;
            XH = (byte)(value >> 8);
            XL = (byte)value;
        }

        public void SetIY(ushort value)
        {
            IY = value;
            YH = (byte)(value >> 8);
            YL = (byte)value;
        }

        public void AddSymbol(string symbol, uint value)
        {
            _symbols[symbol] = value;
        }
    }
}