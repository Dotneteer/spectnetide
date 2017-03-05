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
 * 
 */
using System;


namespace ZXMAK2.Engine.Cpu
{
    /// <summary>
    /// Represent masks for the Z80 flags register
    /// </summary>
    public static class CpuFlags
    {
        #region Flags

        /// <summary>
        /// Sign Flag 
        /// [Set if the 2-complement value is negative (copy of MSB)]
        /// </summary>
        public const byte S = 0x80;
        
        /// <summary>
        /// Zero Flag 
        /// [Set if the value is zero]
        /// </summary>
        public const byte Z = 0x40;

        /// <summary>
        /// Undocumented flag F5
        /// </summary>
        public const byte F5 = 0x20;
        
        /// <summary>
        /// Half Carry Flag 
        /// [Carry from bit 3 to bit 4]
        /// </summary>
        public const byte H = 0x10;

        /// <summary>
        /// Undocumented flag F3
        /// </summary>
        public const byte F3 = 0x08;

        /// <summary>
        /// P/V - Parity/Overflow Flag
        /// [Parity set if even number of bits set.
        /// Overflow set if the 2-complement result does not fit in the register]
        /// </summary>
        public const byte P = 0x04;
        
        /// <summary>
        /// Add/Subtract Flag 
        /// [Set if the last operation was a subtraction]
        /// </summary>
        public const byte N = 0x02;

        /// <summary>
        /// Carry Flag
        /// [Set if the result did not fit in the register]
        /// </summary>
        public const byte C = 0x01;

        #endregion Flags


        #region Inverted Masks

        public const byte NotS = S ^ 0xFF;
        public const byte NotZ = Z ^ 0xFF;
        public const byte NotF5 = F5 ^ 0xFF;
        public const byte NotH = H ^ 0xFF;
        public const byte NotF3 = F3 ^ 0xFF;
        public const byte NotP = P ^ 0xFF;
        public const byte NotN = N ^ 0xFF;
        public const byte NotC = C ^ 0xFF;

        #endregion Inverted Masks


        #region Internal flag sets (used by engine)

        internal const byte F3F5 = F3 | F5;
        internal const byte SF3F5 = S | F3 | F5;
        internal const byte PCF3F5 = P | C | F3 | F5;
        internal const byte HNCF3F5 = H | N | C | F3 | F5;
        internal const byte HPNF3F5 = H | P | N | F3 | F5;
        internal const byte HC = H | C;
        internal const byte SZP = S | Z | P;
        internal const byte HN = H | N;

        internal const byte NotF3F5 = F3F5 ^ 0xFF;
        internal const byte NotSF3F5 = SF3F5 ^ 0xFF;
        internal const byte NotPCF3F5 = PCF3F5 ^ 0xFF;
        internal const byte NotHNCF3F5 = HNCF3F5 ^ 0xFF;
        internal const byte NotHPNF3F5 = HPNF3F5 ^ 0xFF;
        internal const byte NotHC = HC ^ 0xFF;
        internal const byte NotSZP = SZP ^ 0xFF;
        internal const byte NotHN = HN ^ 0xFF;

        #endregion Internal flag sets (used by engine)
    }
}
