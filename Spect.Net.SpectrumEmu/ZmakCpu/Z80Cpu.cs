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
 *  Description: Z80 CPU Emulator
 *  Date: 13.04.2007
 * 
 */
using System;
using System.Linq;

namespace ZXMAK2.Engine.Cpu.Processor
{
    public partial class Z80Cpu
    {
        public readonly CpuRegs regs = new CpuRegs();
        public CpuType CpuType;
        public int RzxCounter;
        public long Tact;
        public bool HALTED;
        public bool IFF1;
        public bool IFF2;
        public byte IM;
        public bool BINT;       // last opcode was EI or DD/FD prefix (to prevent INT handling)
        public CpuModeIndex FX;
        public CpuModeEx XFX;
        public ushort LPC;      // last opcode PC

        public bool INT;
        public bool NMI;
        public bool RST;
        public byte BUS = 0xFF;     // state of free data bus

        public Action RESET;
        public Action NMIACK_M1;
        public Action INTACK_M1;
        public Func<ushort, byte> RDMEM_M1;
        public Func<ushort, byte> RDMEM;
        public Action<ushort, byte> WRMEM;
        public Func<ushort, byte> RDPORT;
        public Action<ushort, byte> WRPORT;
        public Action<ushort> RDNOMREQ;
        public Action<ushort> WRNOMREQ;

        public Z80Cpu()
        {
            _pairGetters = Enumerable
                .Range(0, 4).Select(regs.CreatePairGetter)
                .ToArray();
            _pairSetters = Enumerable
                .Range(0, 4).Select(regs.CreatePairSetter)
                .ToArray();
            _regGetters = Enumerable
                .Range(0, 8).Select(regs.CreateRegGetter)
                .ToArray();
            _regSetters = Enumerable
                .Range(0, 8).Select(regs.CreateRegSetter)
                .ToArray();
            _alualg = CreateAluAlg();
            _opcodes = CreateOpcodes();
            _opcodesFx = CreateOpcodesFx();
            _opcodesEd = CreateOpcodesEd();
            _opcodesCb = CreateOpcodesCb();
            _opcodesFxCb = CreateOpcodesFxCb();

            regs.AF = 0x00;
            regs.BC = 0x00;
            regs.DE = 0x00;
            regs.HL = 0x00;
            regs._AF = 0x00;
            regs._BC = 0x00;
            regs._DE = 0x00;
            regs._HL = 0x00;
            regs.IX = 0x00;
            regs.IY = 0x00;
            regs.IR = 0x00;
            regs.PC = 0x00;
            regs.SP = 0x00;
            regs.MW = 0x00;
        }


        public void ExecCycle()
        {
            byte cmd = 0;
            if (XFX == CpuModeEx.None && FX == CpuModeIndex.None)
            {
                if (ProcessSignals())
                    return;
                LPC = regs.PC;
                cmd = RDMEM_M1(LPC);
            }
            else
            {
                if (ProcessSignals())
                    return;
                cmd = RDMEM(regs.PC);
            }
            Tact += 3;
            regs.PC++;
            if (XFX == CpuModeEx.Cb)
            {
                BINT = false;
                if (FX != CpuModeIndex.None)
                {
                    // elapsed T: 4, 4, 3
                    // will be T: 4, 4, 3, 5
                    int drel = (sbyte)cmd;

                    regs.MW = FX == CpuModeIndex.Ix ? (ushort)(regs.IX + drel) : (ushort)(regs.IY + drel);
                    cmd = RDMEM(regs.PC); Tact += 3;
                    RDNOMREQ(regs.PC); Tact++;
                    RDNOMREQ(regs.PC); Tact++;

                    regs.PC++;
                    _opcodesFxCb[cmd](cmd, regs.MW);
                }
                else
                {
                    //Refresh();
                    regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                    Tact += 1;
                    RzxCounter++;

                    _opcodesCb[cmd](cmd);
                }
                XFX = CpuModeEx.None;
                FX = CpuModeIndex.None;
            }
            else if (XFX == CpuModeEx.Ed)
            {
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;

                BINT = false;
                var edop = _opcodesEd[cmd];
                if (edop != null)
                {
                    edop(cmd);
                }
                XFX = CpuModeEx.None;
                FX = CpuModeIndex.None;
            }
            else if (cmd == 0xDD)
            {
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;

                FX = CpuModeIndex.Ix;
                BINT = true;
            }
            else if (cmd == 0xFD)
            {
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;

                FX = CpuModeIndex.Iy;
                BINT = true;
            }
            else if (cmd == 0xCB)
            {
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;

                XFX = CpuModeEx.Cb;
                BINT = true;
            }
            else if (cmd == 0xED)
            {
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;

                XFX = CpuModeEx.Ed;
                BINT = true;
            }
            else
            {
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;

                BINT = false;
                var opdo = FX == CpuModeIndex.None ? _opcodes[cmd] : _opcodesFx[cmd];
                if (opdo != null)
                {
                    opdo(cmd);
                }
                FX = CpuModeIndex.None;
            }
        }

        private bool ProcessSignals()
        {
            if (RST)    // RESET
            {
                // 3T
                RESET();
                //Refresh();      //+1T
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;


                FX = CpuModeIndex.None;
                XFX = CpuModeEx.None;
                HALTED = false;

                IFF1 = false;
                IFF2 = false;
                regs.PC = 0;
                regs.IR = 0;
                IM = 0;
                //regs.SP = 0xFFFF;
                //regs.AF = 0xFFFF;

                Tact += 2;      // total should be 3T?
                return true;
            }
            else if (NMI)
            {
                // 11T (5, 3, 3)

                if (HALTED) // workaround for Z80 snapshot halt issue + comfortable debugging
                    regs.PC++;

                // M1
                NMIACK_M1();
                Tact += 4;
                //Refresh();
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;
                RzxCounter++;


                IFF2 = IFF1;
                IFF1 = false;
                HALTED = false;
                regs.SP--;

                // M2
                WRMEM(regs.SP, (byte)(regs.PC >> 8));
                Tact += 3;
                regs.SP--;

                // M3
                WRMEM(regs.SP, (byte)regs.PC);
                regs.PC = 0x0066;
                Tact += 3;

                return true;
            }
            else if (INT && (!BINT) && IFF1)
            {
                // http://www.z80.info/interrup.htm
                // IM0: 13T (7,3,3) [RST]
                // IM1: 13T (7,3,3)
                // IM2: 19T (7,3,3,3,3)

                if (HALTED) // workaround for Z80 snapshot halt issue + comfortable debugging
                    regs.PC++;


                INTACK_M1();
                // M1: 7T = interrupt acknowledgement; SP--
                regs.SP--;
                //if (HALTED) ??
                //    Tact += 2;
                Tact += 4 + 2;
                //Refresh();
                //RzxCounter--;	// fix because INTAK should not be calculated
                regs.R = (byte)(((regs.R + 1) & 0x7F) | (regs.R & 0x80));
                Tact += 1;


                IFF1 = false;
                IFF2 = false; // proof?
                HALTED = false;

                // M2
                WRMEM(regs.SP, (byte)(regs.PC >> 8));   // M2: 3T write PCH; SP--
                regs.SP--;
                Tact += 3;

                // M3
                WRMEM(regs.SP, (byte)regs.PC); // M3: 3T write PCL
                Tact += 3;

                if (IM == 0)        // IM 0: execute instruction taken from BUS with timing T+2???
                {
                    regs.MW = 0x0038; // workaround: just execute #FF
                }
                else if (IM == 1)   // IM 1: execute #FF with timing T+2 (11+2=13T)
                {
                    regs.MW = 0x0038;
                }
                else                // IM 2: VH=reg.I; VL=BUS; PC=[V]
                {
                    // M4
                    ushort adr = (ushort)((regs.IR & 0xFF00) | BUS);
                    regs.MW = RDMEM(adr);               // M4: 3T read VL
                    Tact += 3;

                    // M5
                    regs.MW += (ushort)(RDMEM(++adr) * 0x100);   // M5: 3T read VH, PC=V
                    Tact += 3;
                }
                regs.PC = regs.MW;

                return true;
            }
            return false;
        }
    }
}