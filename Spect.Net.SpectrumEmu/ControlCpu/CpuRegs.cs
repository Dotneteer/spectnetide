/* 
 *  Copyright 2007, 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Description: Z80 CPU Emulator [registers part]
 *  Date: 18.03.2007
 *  
 */
using System;
using System.Runtime.InteropServices;
using ZXMAK2.Engine.Cpu.Processor;


namespace ZXMAK2.Engine.Cpu
{
    [StructLayout(LayoutKind.Explicit)]
    public sealed class CpuRegs
    {
        [FieldOffset(0)]
        public ushort AF = 0;
        [FieldOffset(2)]
        public ushort BC = 0;
        [FieldOffset(4)]
        public ushort DE = 0;
        [FieldOffset(6)]
        public ushort HL = 0;
        [FieldOffset(8)]
        public ushort _AF = 0;
        [FieldOffset(10)]
        public ushort _BC = 0;
        [FieldOffset(12)]
        public ushort _DE = 0;
        [FieldOffset(14)]
        public ushort _HL = 0;
        [FieldOffset(16)]
        public ushort IX = 0;
        [FieldOffset(18)]
        public ushort IY = 0;
        [FieldOffset(20)]
        public ushort IR = 0;
        [FieldOffset(22)]
        public ushort PC = 0;
        [FieldOffset(24)]
        public ushort SP = 0;
        [FieldOffset(26)]
        public ushort MW = 0;    // MEMPTR


        [FieldOffset(1)]
        public byte A;
        [FieldOffset(0)]
        public byte F;
        [FieldOffset(3)]
        public byte B;
        [FieldOffset(2)]
        public byte C;
        [FieldOffset(5)]
        public byte D;
        [FieldOffset(4)]
        public byte E;
        [FieldOffset(7)]
        public byte H;
        [FieldOffset(6)]
        public byte L;
        [FieldOffset(17)]
        public byte XH;
        [FieldOffset(16)]
        public byte XL;
        [FieldOffset(19)]
        public byte YH;
        [FieldOffset(18)]
        public byte YL;
        [FieldOffset(21)]
        public byte I;
        [FieldOffset(20)]
        public byte R;

        [FieldOffset(27)]
        public byte MH;
        [FieldOffset(26)]
        public byte ML;


        #region Access Lamda Generators
        
        // Access field from an object context
        // makes one native instruction lesser

        internal Func<byte> CreateRegGetter(int r)
        {
            switch (r)
            {
                case CpuRegId.B: return () => B;
                case CpuRegId.C: return () => C;
                case CpuRegId.D: return () => D;
                case CpuRegId.E: return () => E;
                case CpuRegId.H: return () => H;
                case CpuRegId.L: return () => L;
                case CpuRegId.A: return () => A;
                case CpuRegId.F: return () => F;
                default: throw new ArgumentOutOfRangeException("r");
            }
        }

        internal Action<byte> CreateRegSetter(int r)
        {
            switch (r)
            {
                case CpuRegId.B: return arg => B = arg;
                case CpuRegId.C: return arg => C = arg;
                case CpuRegId.D: return arg => D = arg;
                case CpuRegId.E: return arg => E = arg;
                case CpuRegId.H: return arg => H = arg;
                case CpuRegId.L: return arg => L = arg;
                case CpuRegId.A: return arg => A = arg;
                case CpuRegId.F: return arg => F = arg;
                default: throw new ArgumentOutOfRangeException("r");
            }
        }

        internal Func<ushort> CreatePairGetter(int rr)
        {
            switch (rr)
            {
                case CpuRegId.Bc: return () => BC;
                case CpuRegId.De: return () => DE;
                case CpuRegId.Hl: return () => HL;
                case CpuRegId.Sp: return () => SP;
                default: throw new ArgumentOutOfRangeException("rr");
            }
        }

        internal Action<ushort> CreatePairSetter(int rr)
        {
            switch (rr)
            {
                case CpuRegId.Bc: return arg => BC = arg;
                case CpuRegId.De: return arg => DE = arg;
                case CpuRegId.Hl: return arg => HL = arg;
                case CpuRegId.Sp: return arg => SP = arg;
                default: throw new ArgumentOutOfRangeException("rr");
            }
        }

        #endregion Access Lamda Generators
    }
}
