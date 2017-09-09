/* 
 *  Copyright 2007, 2011, 2015 Alex Makeev
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
 *  Description: Z80 CPU Emulator [FXCB prefixed opcodes part]
 *  Date: 25.09.2011
 * 
 */


namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        private void FXCB_RLC(byte cmd, ushort adr)       // *RLC r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RLC(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RRC(byte cmd, ushort adr)       // *RRC r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RRC(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RL(byte cmd, ushort adr)        // *RL r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RL(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RR(byte cmd, ushort adr)        // *RR r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RR(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SLA(byte cmd, ushort adr)       // *SLA r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SLA(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SRA(byte cmd, ushort adr)       // *SRA r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SRA(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SLL(byte cmd, ushort adr)       // **SLL r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SLL(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SRL(byte cmd, ushort adr)       // *SRL r
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SRL(val);
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RES(byte cmd, ushort adr)       // *RES   b,r,(IX+drel)
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var m = (byte)~(1 << ((cmd & 0x38) >> 3));
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val &= m;
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SET(byte cmd, ushort adr)       // *SET   b,r,(IX+drel)
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var m = (byte)(1 << ((cmd & 0x38) >> 3));
            var r = cmd & 7;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val |= m;
            _regSetters[r](val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RLCIX(byte cmd, ushort adr)       // RLC (IX+) [23T]
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RLC(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RRCIX(byte cmd, ushort adr)       // RRC (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RRC(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RLIX(byte cmd, ushort adr)        // RL (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RL(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_RRIX(byte cmd, ushort adr)        // RR (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_RR(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SLAIX(byte cmd, ushort adr)       // SLA (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SLA(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SRAIX(byte cmd, ushort adr)       // SRA (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SRA(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SLLIX(byte cmd, ushort adr)       // *SLL (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SLL(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SRLIX(byte cmd, ushort adr)       // SRL (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val = ALU_SRL(val);
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_BITIX(byte cmd, ushort adr)       // BIT (IX+)
        {
            // 20T (4, 4, 3, 5, 4)
            var b = (cmd & 0x38) >> 3;

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            ALU_BITMEM(val, b);
        }

        private void FXCB_RESIX(byte cmd, ushort adr)       // RES (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var m = (byte)~(1 << ((cmd & 0x38) >> 3));

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val &= m;
            WRMEM(adr, val); Tact += 3;
        }

        private void FXCB_SETIX(byte cmd, ushort adr)       // SET (IX+)
        {
            // 23T (4, 4, 3, 5, 4, 3)
            var m = (byte)(1 << ((cmd & 0x38) >> 3));

            var val = RDMEM(adr); Tact += 3;
            RDNOMREQ(adr); Tact++;
            val |= m;
            WRMEM(adr, val); Tact += 3;
        }
    }
}
