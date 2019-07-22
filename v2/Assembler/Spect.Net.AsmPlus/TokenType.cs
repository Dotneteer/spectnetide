namespace Spect.Net.AsmPlus
{
    /// <summary>
    /// This enum represents that available token types
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Error, unknown token type
        /// </summary>
        Error = -2,

        /// <summary>
        /// EOF received
        /// </summary>
        Eof = -1,

        /// <summary>
        /// No token received
        /// </summary>
        None = 0,

        NewLine,
        Identifier,
        Colon, //
        Comment, //
        IfDef,
        IfNdef,
        Define,
        Undef,
        IfMod,
        IfNmod,
        EndIf,
        Else,
        If,
        Include,
        String,
        FString,
        LineDir,
        Comma, //
        Loop,
        While,
        Until,
        Elif,
        Macro,
        Lpar, //
        Rpar, //
        EndMacro,
        Proc,
        EndProc,
        EndLoop,
        Repeat,
        EndWhile,
        IfStmt,
        IfUsed,
        IfNused,
        ElseStmt,
        EndIfStmt,
        For,
        Assign, //
        To,
        Step,
        Next,
        ForNext,
        Break,
        Continue,
        Module,
        EndMod,
        Struct,
        Endst,
        Local,
        GoesTo, //
        OrgPrag,
        XorgPrag,
        EntPrag,
        XEntPrag,
        DisPrag,
        EquPrag,
        VarPrag, //
        DbPrag,
        DwPrag,
        DcPrag,
        DmPrag,
        DnPrag,
        DhPrag,
        SkipPrag,
        ExtPrag,
        DsPrag,
        FbPrag,
        FwPrag,
        ModPrag,
        AlgPrag,
        Trace,
        TraceHex,
        EndSeed,
        DgxPrag,
        ErrorPr,
        IncBin,
        CompareBin,

        Nop, //
        Rlca, //
        Rrca, //
        Rla, //
        Rra, //
        Daa, //
        Cpl, //
        Scf, //
        Ccf, //
        Halt, //
        Exx, //
        Di, //
        Ei, //
        Neg, //
        Retn, //
        Reti, //
        Rld, //
        Rrd, //
        Ldi, //
        Cpi, //
        Ini, //
        Outi, //
        Ldd, //
        Cpd, //
        Ind, //
        Outd, //
        Ldir, //
        Cpir, //
        Inir, //
        Otir, //
        Lddr, //
        Cpdr, //
        Indr, //
        Otdr, //
        Ld, //
        Inc, //
        Dec, //
        Ex, //
        Add, //
        Adc, //
        Sub, //
        Sbc, //
        And, //
        Xor, //
        Or, //
        Cp, // 
        Djnz, //
        Jr, //
        Jp, //
        Call, //
        Ret, //
        Rst, //
        Push, //
        Pop, //
        In, //
        Out, //
        Im, //
        Rlc, //
        Rrc, //
        Rl, //
        Rr, //
        Sla, //
        Sra, //
        Sll, //
        Srl, //
        Bit, //
        Res, //
        Set, //
        Mirror, //
        Test, //
        NextReg, //
        SwapNib, //
        Mul, //
        Outinb, //
        Ldix, //
        Ldirx, //
        Lddx, //
        Lddrx, //
        Pixeldn, //
        Pixelad, //
        Setae, //
        Ldpirx, //
        Ldirscale, //

        Hreg, 
        Lreg,
        A, //
        B, //
        C, //
        D, //
        E, //
        H, //
        L, //
        Xl, //
        Xh, //
        Yl, //
        Yh, //
        I, //
        R, //
        Bc, //
        De, //
        Hl, //
        Sp, //
        Ix, //
        Iy, //
        Af, //
        Af_, //
        Z, //
        Nz, //
        Nc, //
        Po, //
        Pe, //
        P, //
        M, //

        Plus, //
        Minus, //
        Tilde, //
        Exclm, //
        Lsbrac, //
        Rsbrac, //
        MinOp, //
        MaxOp, //
        MulOp, //
        DivOp, //
        ModOp, //
        LshOp, //
        RshOp, //
        LtOp, //
        LteOp, //
        GtOp, //
        GteOp, //
        EqOp, //
        NeqOp, //
        CiEqOp, //
        CiNeqOp, //
        Amp, //
        UpArr, //
        VBar, //
        Qmark, //
        TextOf,
        LTextOd,
        Def,
        IsReg8,
        IsReg8Std,
        IsReg8Spec,
        IsReg8Idx,
        IsReg16,
        IsReg16Std,
        IsReg16Idx,
        IsRegIndirect,
        IsCPort,
        IsIndexedAddr,
        IsCondition,
        IsExpr,

        HexNum,
        DecNum,
        OctNum,
        Char,
        BinNum,
        RealNum,
        BoolLit,
        CurAddr,
        Dot, //
        CurCnt,
        Dcolon, //
        LdBrac, //
        RdBrac, //
        Ws,
        BlComment
    }
}