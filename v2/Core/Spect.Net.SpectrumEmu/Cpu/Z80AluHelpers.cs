// ReSharper disable InconsistentNaming

using System;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This partion of the class provides helper tables and
    /// methods for Z80 ALU operations
    /// </summary>
    public partial class Z80Cpu
    {
        /// <summary>
        /// Provides a table that contains the value of the F register after a 8-bit INC operation
        /// </summary>
        private static byte[] s_IncOpFlags;

        /// <summary>
        /// Provides a table that contains the value of the F register after a 8-bit DEC operation
        /// </summary>
        private static byte[] s_DecOpFlags;

        /// <summary>
        /// Stores the accepted AF results of a DAA operation. The first 8 bits of
        /// the index is the value of A before the DAA operation; the ramining 3 bits
        /// are H, N, and C flags respectively.
        /// The upper 8 bits of the value represent A, the lower 8 bits are for F.
        /// </summary>
        private static ushort[] s_DaaResults;

        /// <summary>
        /// Provides a table that contains the value of the F register after a 8-bit ADD/ADC operation.
        /// </summary>
        private static byte[] s_AdcFlags;

        /// <summary>
        /// Provides a table that contains the value of the F register after a 8-bit SUB/SBC operation.
        /// </summary>
        private static byte[] s_SbcFlags;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU bitwise logic operation (according to the result).
        /// </summary>
        private static byte[] s_AluLogOpFlags;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU RLC operation (according to the result).
        /// </summary>
        private static byte[] s_RlcFlags;

        /// <summary>
        /// Provides a table that contains the result of rotate left operations.
        /// </summary>
        private static byte[] s_RolOpResults;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU RRC operation (according to the result).
        /// </summary>
        private static byte[] s_RrcFlags;

        /// <summary>
        /// Provides a table that contains the result of rotate right operations.
        /// </summary>
        private static byte[] s_RorOpResults;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU RL operation with a previous Carry flag value of 1 (according to the result).
        /// This table supports the ALU SLA operation, too.
        /// </summary>
        private static byte[] s_RlCarry0Flags;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU RL operation with a previous Carry flag value of 1 (according to the result).
        /// </summary>
        private static byte[] s_RlCarry1Flags;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU RL operation with a previous Carry flag value of 1 (according to the result).
        /// This table supports the ALU SRA operation, too.
        /// </summary>
        private static byte[] s_RrCarry0Flags;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU RL operation with a previous Carry flag value of 1 (according to the result).
        /// </summary>
        private static byte[] s_RrCarry1Flags;

        /// <summary>
        /// Provides a table that contains the value of the F register after an
        /// 8-bit ALU SRA operation (according to the result).
        /// </summary>
        private static byte[] s_SraFlags;

        /// <summary>
        /// Provides a table tha defines the functions for ALU operation types
        /// </summary>
        private Action<byte,bool>[] _AluAlgorithms;

        /// <summary>
        /// Initializes the helper tables used for ALU operations
        /// </summary>
        private void InitializeAluTables()
        {
            _AluAlgorithms = new Action<byte, bool>[]
            {
                AluADD,
                AluADC,
                AluSUB,
                AluSBC,
                AluAND,
                AluXOR,
                AluOR,
                AluCP
            };

            // --- 8 bit INC operation flags
            s_IncOpFlags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var oldVal = (byte) b;
                var newVal = (byte) (oldVal + 1);
                var flags =
                    // C is unaffected, we keep it false
                    (newVal & FlagsSetMask.R3) |
                    (newVal & FlagsSetMask.R5) |
                    ((newVal & 0x80) != 0 ? FlagsSetMask.S : 0) |
                    (newVal == 0 ? FlagsSetMask.Z : 0) |
                    ((oldVal & 0x0F) == 0x0F ? FlagsSetMask.H : 0) |
                    (oldVal == 0x7F ? FlagsSetMask.PV : 0);
                // N is false
                s_IncOpFlags[b] = (byte) flags;
            }

            // --- 8 bit DEC operation flags
            s_DecOpFlags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var oldVal = (byte) b;
                var newVal = (byte) (oldVal - 1);
                var flags =
                    // C is unaffected, we keep it false
                    (newVal & FlagsSetMask.R3) |
                    (newVal & FlagsSetMask.R5) |
                    ((newVal & 0x80) != 0 ? FlagsSetMask.S : 0) |
                    (newVal == 0 ? FlagsSetMask.Z : 0) |
                    ((oldVal & 0x0F) == 0x00 ? FlagsSetMask.H : 0) |
                    (oldVal == 0x80 ? FlagsSetMask.PV : 0) |
                    FlagsSetMask.N;
                s_DecOpFlags[b] = (byte) flags;
            }

            // --- DAA flags table
            s_DaaResults = new ushort[0x800];
            for (var b = 0; b < 0x100; b++)
            {
                var hNibble = b >> 4;
                var lNibble = b & 0x0F;

                for (var H = 0; H <= 1; H++)
                {
                    for (var N = 0; N <= 1; N++)
                    {
                        for (var C = 0; C <= 1; C++)
                        {
                            // --- Calculate DIFF and the new value of C Flag
                            var diff = 0x00;
                            var cAfter = 0;
                            if (C == 0)
                            {
                                if (hNibble >= 0 && hNibble <= 9 && lNibble >= 0 && lNibble <= 9)
                                {
                                    diff = H == 0 ? 0x00 : 0x06;
                                }
                                else if (hNibble >= 0 && hNibble <= 8 && lNibble >= 0x0A && lNibble <= 0xF)
                                {
                                    diff = 0x06;
                                }
                                else if (hNibble >= 0x0A && hNibble <= 0x0F && lNibble >= 0 && lNibble <= 9 && H == 0)
                                {
                                    diff = 0x60;
                                    cAfter = 1;
                                }
                                else if (hNibble >= 9 && hNibble <= 0x0F && lNibble >= 0x0A && lNibble <= 0xF)
                                {
                                    diff = 0x66;
                                    cAfter = 1;
                                }
                                else if (hNibble >= 0x0A && hNibble <= 0x0F && lNibble >= 0 && lNibble <= 9)
                                {
                                    if (H == 1) diff = 0x66;
                                    cAfter = 1;
                                }
                            }
                            else
                            {
                                // C == 1
                                cAfter = 1;
                                if (lNibble >= 0 && lNibble <= 9)
                                {
                                    diff = H == 0 ? 0x60 : 0x66;
                                }
                                else if (lNibble >= 0x0A && lNibble <= 0x0F)
                                {
                                    diff = 0x66;
                                }
                            }

                            // --- Calculate new value of H Flag
                            var hAfter = 0;
                            if (lNibble >= 0x0A && lNibble <= 0x0F && N == 0
                                || lNibble >= 0 && lNibble <= 5 && N == 1 && H == 1)
                            {
                                hAfter = 1;
                            }

                            // --- Calculate new value of register A
                            var A = (N == 0 ? b + diff : b - diff) & 0xFF;

                            // --- Calculate other flags
                            var aPar = 0;
                            var val = A;
                            for (var i = 0; i < 8; i++)
                            {
                                aPar += val & 0x01;
                                val >>= 1;
                            }

                            // --- Calculate result
                            var fAfter =
                                (A & FlagsSetMask.R3) |
                                (A & FlagsSetMask.R5) |
                                ((A & 0x80) != 0 ? FlagsSetMask.S : 0) |
                                (A == 0 ? FlagsSetMask.Z : 0) |
                                (aPar % 2 == 0 ? FlagsSetMask.PV : 0) |
                                (N == 1 ? FlagsSetMask.N : 0) |
                                (hAfter == 1 ? FlagsSetMask.H : 0) |
                                (cAfter == 1 ? FlagsSetMask.C : 0);

                            var result = (ushort) (A << 8 | (byte) fAfter);
                            var fBefore = (byte) (H * 4 + N * 2 + C);
                            var idx = (fBefore << 8) + b;
                            s_DaaResults[idx] = result;
                        }
                    }
                }
            }

            // --- ADD and ADC flags
            s_AdcFlags = new byte[0x20000];
            for (var C = 0; C < 2; C++)
            {
                for (var X = 0; X < 0x100; X++)
                {
                    for (var Y = 0; Y < 0x100; Y++)
                    {
                        var res = (ushort) (X + Y + C);
                        var flags = 0;
                        if ((res & 0xFF) == 0) flags |= FlagsSetMask.Z;
                        flags |= res & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S);
                        if (res >= 0x100) flags |= FlagsSetMask.C;
                        if ((((X & 0x0F) + (Y & 0x0F) + C) & 0x10) != 0) flags |= FlagsSetMask.H;
                        var ri = (sbyte) X + (sbyte) Y + C;
                        if (ri >= 0x80 || ri <= -0x81) flags |= FlagsSetMask.PV;
                        s_AdcFlags[C * 0x10000 + X * 0x100 + Y] = (byte) flags;
                    }
                }
            }

            // --- SUB and SBC flags
            s_SbcFlags = new byte[0x20000];
            for (var C = 0; C < 2; C++)
            {
                for (var X = 0; X < 0x100; X++)
                {
                    for (var Y = 0; Y < 0x100; Y++)
                    {
                        var res = X - Y - C;
                        var flags = res & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S);
                        if ((res & 0xFF) == 0) flags |= FlagsSetMask.Z;
                        if ((res & 0x10000) != 0) flags |= FlagsSetMask.C;
                        var ri = (sbyte) X - (sbyte) Y - C;
                        if (ri >= 0x80 || ri < -0x80) flags |= FlagsSetMask.PV;
                        if ((((X & 0x0F) - (res & 0x0F) - C) & 0x10) != 0) flags |= FlagsSetMask.H;
                        flags |= FlagsSetMask.N;
                        s_SbcFlags[C * 0x10000 + X * 0x100 + Y] = (byte) flags;
                    }
                }
            }

            // --- ALU log operation (AND, XOR, OR) flags
            s_AluLogOpFlags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var fl = b & (FlagsSetMask.R3 | FlagsSetMask.R5 | FlagsSetMask.S);
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((b & i) != 0) p ^= FlagsSetMask.PV;
                }
                s_AluLogOpFlags[b] = (byte)(fl | p);
            }
            s_AluLogOpFlags[0] |= FlagsSetMask.Z;

            // --- 8-bit RLC operation flags
            s_RlcFlags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var rlcVal = b;
                rlcVal <<= 1;
                var cf = (rlcVal & 0x100) != 0 ? FlagsSetMask.C : 0;
                if (cf != 0)
                {
                    rlcVal = (rlcVal | 0x01) & 0xFF;
                }
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((rlcVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(rlcVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if (rlcVal == 0) flags |= FlagsSetMask.Z;
                s_RlcFlags[b] = flags;
            }

            // --- 8-bit RRC operation flags
            s_RrcFlags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var rrcVal = b;
                var cf = (rrcVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                rrcVal >>= 1;
                if (cf != 0)
                {
                    rrcVal = (rrcVal | 0x80);
                }
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((rrcVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(rrcVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if (rrcVal == 0) flags |= FlagsSetMask.Z;
                s_RrcFlags[b] = flags;
            }

            // --- 8-bit RL operations with 0 Carry flag
            s_RlCarry0Flags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var rlVal = b;
                rlVal <<= 1;
                var cf = (rlVal & 0x100) != 0 ? FlagsSetMask.C : 0;
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((rlVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(rlVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if ((rlVal & 0xFF) == 0)
                {
                    flags |= FlagsSetMask.Z;
                }
                s_RlCarry0Flags[b] = flags;
            }

            // --- 8-bit RL operations with Carry flag set
            s_RlCarry1Flags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var rlVal = b;
                rlVal <<= 1;
                rlVal++;
                var cf = (rlVal & 0x100) != 0 ? FlagsSetMask.C : 0;
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((rlVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(rlVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if ((rlVal & 0x1FF) == 0)
                {
                    flags |= FlagsSetMask.Z;
                }
                s_RlCarry1Flags[b] = flags;
            }

            // --- 8-bit RR operations with 0 Carry flag
            s_RrCarry0Flags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var rrVal = b;
                var cf = (rrVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                rrVal >>= 1;
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((rrVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(rrVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if (rrVal == 0) flags |= FlagsSetMask.Z;
                s_RrCarry0Flags[b] = flags;
            }

            // --- 8-bit RR operations with Carry flag set
            s_RrCarry1Flags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var rrVal = b;
                var cf = (rrVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                rrVal >>= 1;
                rrVal += 0x80;
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((rrVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(rrVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if (rrVal == 0) flags |= FlagsSetMask.Z;
                s_RrCarry1Flags[b] = flags;
            }

            // --- 8-bit SRA operation flags
            s_SraFlags = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var sraVal = b;
                var cf = (sraVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                sraVal = (sraVal >> 1) + (sraVal & 0x80);
                var p = FlagsSetMask.PV;
                for (var i = 0x80; i != 0; i /= 2)
                {
                    if ((sraVal & i) != 0) p ^= FlagsSetMask.PV;
                }
                var flags = (byte)(sraVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3) | p | cf);
                if ((sraVal & 0xFF) == 0) flags |= FlagsSetMask.Z;
                s_SraFlags[b] = flags;
            }

            // --- Initialize rotate operation tables
            s_RolOpResults = new byte[0x100];
            s_RorOpResults = new byte[0x100];

            for (var b = 0; b < 0x100; b++)
            {
                s_RolOpResults[b] = (byte)((b << 1) + (b >> 7));
                s_RorOpResults[b] = (byte)((b >> 1) + (b << 7));
            }
        }

        /// <summary>
        /// Increments the specified value and sets F according to INC ALU logic
        /// </summary>
        /// <param name="val">Value to increment</param>
        /// <returns>Incremented value</returns>
        private byte AluIncByte(byte val)
        {
            _registers.F = (byte)(s_IncOpFlags[val] | _registers.F & FlagsSetMask.C);
            val++;
            return val;
        }

        /// <summary>
        /// Increments the specified value and sets F according to INC ALU logic
        /// </summary>
        /// <param name="val">Value to increment</param>
        /// <returns>Incremented value</returns>
        private byte AluDecByte(byte val)
        {
            _registers.F = (byte)(s_DecOpFlags[val] | _registers.F & FlagsSetMask.C);
            val--;
            return val;
        }

        /// <summary>
        /// Adds the <paramref name="regHL"/> value and <paramref name="regOther"/> value
        /// according to the rule of ADD HL,QQ operation
        /// </summary>
        /// <param name="regHL">HL (IX, IY) value</param>
        /// <param name="regOther">Other value</param>
        /// <returns>Result value</returns>
        private ushort AluAddHL(ushort regHL, ushort regOther)
        {
            // --- Keep unaffected flags
            _registers.F = (byte)(_registers.F & ~(FlagsSetMask.N | FlagsSetMask.C 
                                                 | FlagsSetMask.R5 | FlagsSetMask.R3 | FlagsSetMask.H));

            // --- Calculate Carry from bit 11
            _registers.F |= (byte)((((regHL & 0x0FFF) + (regOther & 0x0FFF)) >> 8) & FlagsSetMask.H);
            var res = (uint)((regHL & 0xFFFF) + (regOther & 0xFFFF));

            // --- Calculate Carry
            if ((res & 0x10000) != 0) _registers.F |= FlagsSetMask.C;

            // --- Set R5 and R3 according to the low 8-bit of result
            _registers.F |= (byte)((byte)((res >> 8) & 0xFF) & (FlagsSetMask.R5 | FlagsSetMask.R3));
            return (ushort)(res & 0xFFFF);
        }

        /// <summary>
        /// Executes the ADD operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluADD(byte right, bool cf)
        {
            AluADC(right, false);
        }

        /// <summary>
        /// Executes the ADC operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluADC(byte right, bool cf)
        {
            var c = cf ? 1 : 0;
            var result = _registers.A + right + c;
            var signed = (sbyte)_registers.A + (sbyte)right + c;
            var lNibble = ((_registers.A & 0x0F) + (right & 0x0F) + c) & 0x10;

            var flags = (byte)(result & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
            if ((result & 0xFF) == 0) flags |= FlagsSetMask.Z;
            if (result >= 0x100) flags |= FlagsSetMask.C;
            if (lNibble != 0) flags |= FlagsSetMask.H;
            if (signed >= 0x80 || signed <= -0x81) flags |= FlagsSetMask.PV;

            _registers.F = flags;
            _registers.A = (byte) result;
        }

        /// <summary>
        /// Executes the SUB operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluSUB(byte right, bool cf)
        {
            AluSBC(right, false);
        }

        /// <summary>
        /// Executes the SBC operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluSBC(byte right, bool cf)
        {
            var c = cf ? 1 : 0;
            var result = _registers.A - right - c;
            var signed = (sbyte)_registers.A - (sbyte)right - c;
            var lNibble = ((_registers.A & 0x0F) - (right & 0x0F) - c) & 0x10;

            var flags = (byte)(result & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
            flags |= FlagsSetMask.N;
            if ((result & 0xFF) == 0) flags |= FlagsSetMask.Z;
            if ((result & 0x10000) != 0) flags |= FlagsSetMask.C;
            if (lNibble != 0) flags |= FlagsSetMask.H;
            if (signed >= 0x80 || signed <= -0x81) flags |= FlagsSetMask.PV;

            _registers.F = flags;
            _registers.A = (byte)result;
        }

        /// <summary>
        /// Executes the AND operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluAND(byte right, bool cf)
        {
            _registers.A &= right;
            _registers.F = (byte)(s_AluLogOpFlags[_registers.A] | FlagsSetMask.H);
        }

        /// <summary>
        /// Executes the XOR operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluXOR(byte right, bool cf)
        {
            _registers.A ^= right;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        /// Executes the OR operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluOR(byte right, bool cf)
        {
            _registers.A |= right;
            _registers.F = s_AluLogOpFlags[_registers.A];
        }

        /// <summary>
        /// Executes the CP operation.
        /// </summary>
        /// <param name="right">Right operand</param>
        /// <param name="cf">Carry flag</param>
        private void AluCP(byte right, bool cf)
        {
            var result = _registers.A - right;
            var signed = (sbyte)_registers.A - (sbyte)right;
            var lNibble = ((_registers.A & 0x0F) - (right & 0x0F)) & 0x10;

            var flags = (byte)(result & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
            flags |= FlagsSetMask.N;
            if ((result & 0xFF) == 0) flags |= FlagsSetMask.Z;
            if ((result & 0x10000) != 0) flags |= FlagsSetMask.C;
            if (lNibble != 0) flags |= FlagsSetMask.H;
            if (signed >= 0x80 || signed <= -0x81) flags |= FlagsSetMask.PV;

            _registers.F = flags;
        }
    }
}