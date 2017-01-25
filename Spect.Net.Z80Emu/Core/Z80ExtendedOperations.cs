// ReSharper disable InconsistentNaming

using System;

namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// This partion of the class provides extended operations
    /// (OpCodes used with the 0xED prefix)
    /// </summary>
    public partial class Z80
    {
        /// <summary>
        /// Extended (0xED-prefixed) operations jump table
        /// </summary>
        private Action<byte>[] _extendedOperations;

        /// <summary>
        /// Processes the operations with 0xED prefix
        /// </summary>
        /// <param name="opCode">Operation code</param>
        private void ProcessEDOperations(byte opCode)
        {
            var opMethod = _extendedOperations[opCode];
            opMethod?.Invoke(opCode);
        }

        /// <summary>
        /// Initializes the extended operation execution tables
        /// </summary>
        private void InitializeExtendedOpsExecutionTable()
        {
            _extendedOperations = new Action<byte>[]
            {
                null,     null,     null,     null,     null,     null,     null,     null,     // 00..07
                null,     null,     null,     null,     null,     null,     null,     null,     // 08..0F
                null,     null,     null,     null,     null,     null,     null,     null,     // 10..17
                null,     null,     null,     null,     null,     null,     null,     null,     // 18..1F
                null,     null,     null,     null,     null,     null,     null,     null,     // 20..27
                null,     null,     null,     null,     null,     null,     null,     null,     // 28..2F
                null,     null,     null,     null,     null,     null,     null,     null,     // 30..37
                null,     null,     null,     null,     null,     null,     null,     null,     // 38..3F

                IN_R_P,   OUT_P_R,  SBC_HLRR, LD_NNiRR, NEG,      RETN,     IM_N,     LD_XR_A,  // 40..47
                IN_R_P,   OUT_P_R,  ADC_HLRR, LD_RRNNi, NEG,      RETN,     IM_N,     LD_XR_A,  // 48..4F
                IN_R_P,   OUT_P_R,  SBC_HLRR, LD_NNiRR, NEG,      RETN,     IM_N,     LD_A_XR,  // 50..57
                IN_R_P,   OUT_P_R,  ADC_HLRR, LD_RRNNi, NEG,      RETN,     IM_N,     LD_A_XR,  // 58..5F
                IN_R_P,   OUT_P_R,  SBC_HLRR, LD_NNiRR, NEG,      RETN,     IM_N,     RRD,      // 60..67
                IN_R_P,   OUT_P_R,  ADC_HLRR, LD_RRNNi, NEG,      RETN,     IM_N,     RLD,      // 60..6F
                IN_R_P,   OUT_P_R,  SBC_HLRR, LD_NNiRR, NEG,      RETN,     IM_N,     null,     // 70..77
                IN_R_P,   OUT_P_R,  ADC_HLRR, LD_RRNNi, NEG,      RETN,     IM_N,     null,     // 78..7F

                null,     null,     null,     null,     null,     null,     null,     null,     // 80..87
                null,     null,     null,     null,     null,     null,     null,     null,     // 88..8F
                null,     null,     null,     null,     null,     null,     null,     null,     // 90..97
                null,     null,     null,     null,     null,     null,     null,     null,     // 98..9F
                LDI,      CPI,      INI,      OUTI,     null,     null,     null,     null,     // A0..A7
                LDD,      CPD,      IND,      OUTD,     null,     null,     null,     null,     // A8..AF
                LDIR,     CPIR,     INIR,     OTIR,     null,     null,     null,     null,     // B0..B7
                LDDR,     CPDR,     INDR,     OTDR,     null,     null,     null,     null,     // B0..BF

                null,     null,     null,     null,     null,     null,     null,     null,     // C0..C7
                null,     null,     null,     null,     null,     null,     null,     null,     // C8..CF
                null,     null,     null,     null,     null,     null,     null,     null,     // D0..D7
                null,     null,     null,     null,     null,     null,     null,     null,     // D8..DF
                null,     null,     null,     null,     null,     null,     null,     null,     // E0..E7
                null,     null,     null,     null,     null,     null,     null,     null,     // E8..EF
                null,     null,     null,     null,     null,     null,     null,     null,     // F0..F7
                null,     null,     null,     null,     null,     null,     null,     null,     // F8..FF
            };
        }

        private void RRD(byte obj)
        {
            throw new NotImplementedException();
        }

        private void OTDR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INDR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void CPDR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LDDR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void OTIR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INIR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void CPIR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LDIR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void OUTD(byte obj)
        {
            throw new NotImplementedException();
        }

        private void IND(byte obj)
        {
            throw new NotImplementedException();
        }

        private void CPD(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LDD(byte obj)
        {
            throw new NotImplementedException();
        }

        private void OUTI(byte obj)
        {
            throw new NotImplementedException();
        }

        private void INI(byte obj)
        {
            throw new NotImplementedException();
        }

        private void CPI(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LDI(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RLD(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_A_XR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_RRNNi(byte obj)
        {
            throw new NotImplementedException();
        }

        private void ADC_HLRR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_XR_A(byte obj)
        {
            throw new NotImplementedException();
        }

        private void IM_N(byte obj)
        {
            throw new NotImplementedException();
        }

        private void RETN(byte obj)
        {
            throw new NotImplementedException();
        }

        private void NEG(byte obj)
        {
            throw new NotImplementedException();
        }

        private void LD_NNiRR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void SBC_HLRR(byte obj)
        {
            throw new NotImplementedException();
        }

        private void OUT_P_R(byte obj)
        {
            throw new NotImplementedException();
        }

        private void IN_R_P(byte obj)
        {
            throw new NotImplementedException();
        }
    }
}