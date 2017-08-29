using System.Collections.Generic;
using AntlrZ80Asm.SyntaxTree;
using AntlrZ80Asm.SyntaxTree.Operations;
// ReSharper disable InlineOutVariableDeclaration

// ReSharper disable UsePatternMatching

namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public partial class Z80Assembler
    {
        private CompilationSegment _currentSegment;

		/// <summary>
        /// Emits the code after processing the directives
        /// </summary>
        /// <returns></returns>
        private bool EmitCode()
        {
			// --- Initialize code emission
			_output.Segments.Clear();

            foreach (var asmLine in PreprocessedLines)
            {
                var pragmaLine = asmLine as PragmaBase;
                if (pragmaLine != null)
                {
                    ApplyPragma(pragmaLine);
                }
                else
                {
                    var opLine = asmLine as OperationBase;
                    if (opLine != null)
                    {
                        EmitCodeFor(opLine);
                    }
                    else
                    {
						_output.Errors.Add(new UnexpectedSourceCodeLineError(asmLine, 
							$"A pragma or an operation line was expected, but a {asmLine.GetType()} line received"));
                    }
                }
            }
            return _output.ErrorCount == 0;
        }

        private void ApplyPragma(PragmaBase pragmaLine)
        {
        }

		/// <summary>
        /// Emits code for the specified operation
        /// </summary>
        /// <param name="opLine"></param>
        private void EmitCodeFor(OperationBase opLine)
		{
		    if (opLine is TrivialOperation)
		    {
		        EmitTrivialOperation(opLine);
		    }
		}

		/// <summary>
        /// Emits a trivial operation
        /// </summary>
        /// <param name="trivialOp"></param>
        private void EmitTrivialOperation(OperationBase trivialOp)
		{
		    int code;
		    if (s_TrivialOpBytes.TryGetValue(trivialOp.Mnemonic, out code))
		    {
		        var low = (byte)(code & 0xFF);
		        var high = (byte) ((code >> 8) & 0xFF);
		        if (high != 0)
		        {
		            EmitByte(high);
		        }
		        EmitByte(low);
		        return;
		    }
			_output.Errors.Add(new UnexpectedSourceCodeLineError(trivialOp, 
				$"Cannot find code for trivial operation '{trivialOp.Mnemonic}'"));
		}

        /// <summary>
        /// Emits a new byte to the current code segment
        /// </summary>
        /// <param name="data">Data byte to emit</param>
        /// <returns>Current code offset</returns>
        private int EmitByte(byte data)
        {
            if (_currentSegment == null)
            {
                _currentSegment = new CompilationSegment
                {
                    StartAddress = _options.DefaultStartAddress ?? 0x8000,
                    Displacement = _options.DefaultDisplacement ?? 0x0000
                };
				_output.Segments.Add(_currentSegment);
            }
			_currentSegment.EmittedCode.Add(data);
            return _currentSegment.CurrentOffset;
        }

        private static readonly Dictionary<string, int> s_TrivialOpBytes = new Dictionary<string, int>
        {
            {"NOP",  0x00},
            {"RLCA", 0x07},
            {"RRCA", 0x0F},
            {"RLA", 0x17},
            {"RRA", 0x1F},
            {"DAA", 0x27},
            {"CPL", 0x2F},
            {"SCF", 0x37},
            {"CCF", 0x3F},
            {"RET", 0xC9},
            {"EXX", 0xD9},
            {"DI", 0xF3},
            {"EI", 0xFB},
            {"NEG", 0xED44},
            {"RETN", 0xED45},
            {"RETI", 0xED4D},
            {"RRD", 0xED67},
            {"RLD", 0xED6F},
            {"LDI", 0xEDA0},
            {"CPI", 0xEDA1},
            {"INI", 0xEDA2},
            {"OUTI", 0xEDA3},
            {"LDD", 0xEDA8},
            {"CPD", 0xEDA9},
            {"IND", 0xEDAA},
            {"OUTD", 0xEDAB},
            {"LDIR", 0xEDB0},
            {"CPIR", 0xEDB1},
            {"INIR", 0xEDB2},
            {"OTIR", 0xEDB3},
            {"LDDR", 0xEDB8},
            {"CPDR", 0xEDB9},
            {"INDR", 0xEDBA},
            {"OTDR", 0xEDBB}
        };
    }
}