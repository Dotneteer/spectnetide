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
 *  Description: Z80 CPU Emulator [CB prefixed opcodes part]
 *  Date: 13.04.2007
 * 
 */


namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        private void CB_RLC(byte cmd)       // RLC r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_RLC(_regGetters[r]()));
        }

        private void CB_RRC(byte cmd)       // RRC r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_RRC(_regGetters[r]()));
        }

        private void CB_RL(byte cmd)        // RL r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_RL(_regGetters[r]()));
        }

        private void CB_RR(byte cmd)        // RR r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_RR(_regGetters[r]()));
        }

        private void CB_SLA(byte cmd)       // SLA r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_SLA(_regGetters[r]()));
        }

        private void CB_SRA(byte cmd)       // SRA r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_SRA(_regGetters[r]()));
        }

        private void CB_SLL(byte cmd)       // *SLL r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_SLL(_regGetters[r]()));
        }

        private void CB_SRL(byte cmd)       // SRL r
        {
            var r = cmd & 7;
            _regSetters[r](ALU_SRL(_regGetters[r]()));
        }

        private void CB_BIT(byte cmd)       // BIT r
        {
            var r = cmd & 7;
            var b = (cmd & 0x38) >> 3;

            ALU_BIT(_regGetters[r](), b);
        }

        private void CB_RES(byte cmd)       // RES r
        {
            var r = cmd & 7;
            var m = ~(1 << ((cmd & 0x38) >> 3));

            _regSetters[r]((byte)(_regGetters[r]() & m));
        }

        private void CB_SET(byte cmd)       // SET r
        {
            var r = cmd & 7;
            var m = 1 << ((cmd & 0x38) >> 3);

            _regSetters[r]((byte)(_regGetters[r]() | m));
        }

        private void CB_RLCHL(byte cmd)       // RLC (HL)
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_RLC(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_RRCHL(byte cmd)       // RRC (HL)
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_RRC(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_RLHL(byte cmd)        // RL (HL)
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_RL(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_RRHL(byte cmd)        // RR (HL) [15T]
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_RR(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_SLAHL(byte cmd)       // SLA (HL) [15T]
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_SLA(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_SRAHL(byte cmd)       // SRA (HL) [15T]
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_SRA(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_SLLHL(byte cmd)       // *SLL (HL) [15T]
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_SLL(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_SRLHL(byte cmd)       // SRL (HL)
        {
            // 15T (4, 4, 4, 3)

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val = ALU_SRL(val);
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_BITHL(byte cmd)       // BIT (HL) [12T]
        {
            // 12T (4, 4, 4)
            var b = (cmd & 0x38) >> 3;

            var val = RDMEM(regs.HL); Tact += 4;
            ALU_BITMEM(val, b);
        }

        private void CB_RESHL(byte cmd)       // RES (HL) [15T]
        {
            // 15 (4, 4, 4, 3)
            var m = (byte)~(1 << ((cmd & 0x38) >> 3));

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val &= m;
            WRMEM(regs.HL, val); Tact += 3;
        }

        private void CB_SETHL(byte cmd)       // SET (HL) [15T]
        {
            // 15T (4, 4, 4, 3)
            var m = (byte)(1 << ((cmd & 0x38) >> 3));

            var val = RDMEM(regs.HL); Tact += 3;
            RDNOMREQ(regs.HL); Tact++;
            val |= m;
            WRMEM(regs.HL, val); Tact += 3;
        }
    }
}
