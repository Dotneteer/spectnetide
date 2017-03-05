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
 *  Description: Z80 CPU Emulator [ALU part]
 *  Date: 13.04.2007
 * 
 */


namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        #region ALU

        private void ALU_ADDR(byte src)
        {
            regs.F = CpuTables.Adcf[regs.A + src * 0x100];
            regs.A += src;
        }
        
        private void ALU_ADCR(byte src)
        {
            var carry = regs.F & CpuFlags.C;
            regs.F = CpuTables.Adcf[regs.A + src * 0x100 + 0x10000 * carry];
            regs.A += (byte)(src + carry);
        }
        
        private void ALU_SUBR(byte src)
        {
            regs.F = CpuTables.Sbcf[regs.A * 0x100 + src];
            regs.A -= src;
        }
        
        private void ALU_SBCR(byte src)
        {
            var carry = regs.F & CpuFlags.C;
            regs.F = CpuTables.Sbcf[regs.A * 0x100 + src + 0x10000 * carry];
            regs.A -= (byte)(src + carry);
        }
        
        private void ALU_ANDR(byte src)
        {
            regs.A &= src;
            regs.F = (byte)(CpuTables.Logf[regs.A] | CpuFlags.H);
        }
        
        private void ALU_XORR(byte src)
        {
            regs.A ^= src;
            regs.F = CpuTables.Logf[regs.A];
        }
        
        private void ALU_ORR(byte src)
        {
            regs.A |= src;
            regs.F = CpuTables.Logf[regs.A];
        }
        
        private void ALU_CPR(byte src)
        {
            regs.F = CpuTables.Cpf[regs.A * 0x100 + src];
        }

        private byte ALU_INCR(byte x)
        {
            regs.F = (byte)(CpuTables.Incf[x] | (regs.F & CpuFlags.C));
            x++;
            return x;
        }
        
        private byte ALU_DECR(byte x)
        {
            regs.F = (byte)(CpuTables.Decf[x] | (regs.F & CpuFlags.C));
            x--;
            return x;
        }

        private ushort ALU_ADDHLRR(ushort rhl, ushort rde)
        {
            regs.F = (byte)(regs.F & CpuFlags.NotHNCF3F5);
            regs.F |= (byte)((((rhl & 0x0FFF) + (rde & 0x0FFF)) >> 8) & CpuFlags.H);
            uint res = (uint)((rhl & 0xFFFF) + (rde & 0xFFFF));

            if ((res & 0x10000) != 0) regs.F |= CpuFlags.C;
            regs.F |= (byte)((res >> 8) & CpuFlags.F3F5);
            return (ushort)res;
        }

        #endregion

        #region ALU #CB

        private byte ALU_RLC(int x)
        {
            regs.F = CpuTables.Rlcf[x];
            x <<= 1;
            if ((x & 0x100) != 0) x |= 0x01;
            return (byte)x;
        }
        
        private byte ALU_RRC(int x)
        {
            regs.F = CpuTables.Rrcf[x];
            if ((x & 0x01) != 0) x = (x >> 1) | 0x80;
            else x >>= 1;
            return (byte)x;
        }
        
        private byte ALU_RL(int x)
        {
            if ((regs.F & CpuFlags.C) != 0)
            {
                regs.F = CpuTables.Rl1[x];
                x <<= 1;
                x++;
            }
            else
            {
                regs.F = CpuTables.Rl0[x];
                x <<= 1;
            }
            return (byte)x;
        }
        
        private byte ALU_RR(int x)
        {
            if ((regs.F & CpuFlags.C) != 0)
            {
                regs.F = CpuTables.Rr1[x];
                x >>= 1;
                x += 0x80;
            }
            else
            {
                regs.F = CpuTables.Rr0[x];
                x >>= 1;
            }
            return (byte)x;
        }
        
        private byte ALU_SLA(int x)
        {
            regs.F = CpuTables.Rl0[x];
            x <<= 1;
            return (byte)x;
        }
        
        private byte ALU_SRA(int x)
        {
            regs.F = CpuTables.Sraf[x];
            x = (x >> 1) + (x & 0x80);
            return (byte)x;
        }
        
        private byte ALU_SLL(int x)
        {
            regs.F = CpuTables.Rl1[x];
            x <<= 1;
            x++;
            return (byte)x;
        }
        
        private byte ALU_SRL(int x)
        {
            regs.F = CpuTables.Rr0[x];
            x >>= 1;
            return (byte)x;
        }
        
        private void ALU_BIT(byte src, int bit)
        {
            regs.F = (byte)(CpuTables.Logf[src & (1 << bit)] | CpuFlags.H | (regs.F & CpuFlags.C) | (src & CpuFlags.F3F5));
        }
        
        private void ALU_BITMEM(byte src, int bit)
        {
            regs.F = (byte)(CpuTables.Logf[src & (1 << bit)] | CpuFlags.H | (regs.F & CpuFlags.C));
            regs.F = (byte)((regs.F & CpuFlags.NotF3F5) | (regs.MH & CpuFlags.F3F5));
        }

        #endregion
    }
}
