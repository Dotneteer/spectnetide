using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class DirectiveTests
    {
        [TestMethod]
        public void NoPreprocessorDoesNotChangesLine()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            const string SOURCE = @"nop
                  nop
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(3);
        }

        [TestMethod]
        public void DefineAddsNewConditional()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            const string SOURCE = @"nop
                  #define MySymbol
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
            compiler.ConditionSymbols.ShouldContain(s => s == "MYSYMBOL");
        }

        [TestMethod]
        public void DefineKeepsExistingConditional()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"nop
                  #define MySymbol
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
            compiler.ConditionSymbols.ShouldContain(s => s == "MYSYMBOL");
        }

        [TestMethod]
        public void UndefRemovesExistingConditional()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"nop
                  #undef MySymbol
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
            compiler.ConditionSymbols.ShouldNotContain(s => s == "MYSYMBOL");
        }

        [TestMethod]
        public void UndefKeepsUndefinedConditional()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            const string SOURCE = @"nop
                  #undef MySymbol
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
            compiler.ConditionSymbols.ShouldNotContain(s => s == "MYSYMBOL");
        }

        [TestMethod]
        public void IfdefTrueWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  nop ; 1
                  #ifdef MySymbol
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfdefFalseWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  nop ; 1
                  #ifdef MySymbol
                  nop
                  nop
                  nop
                  #endif
                  nop ; 2";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
        }

        [TestMethod]
        public void IfdefTrueWorksWithElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  nop ;
                  #ifdef MySymbol
                  nop ;
                  nop ;
                  nop ;
                  #else
                  nop
                  nop
                  #endif
                  nop ;";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfdefFalseWorksWithElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  nop ; 1
                  #ifdef MySymbol
                  nop
                  nop
                  nop
                  #else
                  nop ; 2
                  nop ; 3
                  #endif
                  nop ; 4";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(4);
        }

        [TestMethod]
        public void IfndefTrueWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  nop ; 1
                  #ifndef MySymbol
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfndefFalseWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  nop ; 1
                  #ifndef MySymbol
                  nop
                  nop
                  nop
                  #endif
                  nop ; 2";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
        }

        [TestMethod]
        public void IfndefTrueWorksWithElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  nop ; 1
                  #ifndef MySymbol
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #else
                  nop
                  nop
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfndefFalseWorksWithElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  nop ; 1
                  #ifndef MySymbol
                  nop
                  nop
                  nop
                  #else
                  nop ; 2
                  nop ; 3
                  #endif
                  nop ; 4";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(4);
        }

        [TestMethod]
        public void UnexpectedElseRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"nop
                  #else ; 1
                  nop
                  nop
                  nop
                  #else ; 2
                  nop
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0060);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0060);
            compiler.PreprocessedLines.Count.ShouldBe(6);
        }

        [TestMethod]
        public void UnexpectedEndifRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"nop
                  #endif ; 1
                  nop
                  nop
                  nop
                  #endif ; 2
                  nop
                  nop";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(2);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0061);
            output.Errors[1].ErrorCode.ShouldBe(Errors.Z0061);
            compiler.PreprocessedLines.Count.ShouldBe(6);
        }

        [TestMethod]
        public void NestedIfdefTrueTrueWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  nop ; 2
                  #ifdef MySymbol2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #endif
                  nop ; 6
                  nop ; 7
                  nop ; 8 
                  nop ; 9
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(9);
        }

        [TestMethod]
        public void NestedIfdefTrueFalseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  nop ; 2
                  #ifdef MySymbol2
                  nop
                  nop
                  nop
                  #endif
                  nop ; 3
                  nop ; 4
                  nop ; 5 
                  nop ; 6
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(6);
        }

        [TestMethod]
        public void NestedIfdefFalseTrueWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop
                  nop
                  #ifdef MySymbol2
                  nop
                  nop
                  nop
                  #endif
                  nop
                  nop
                  nop
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(0);
        }

        [TestMethod]
        public void NestedIfdefFalseFalseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop
                  nop
                  #ifdef MySymbol2
                  nop
                  nop
                  nop
                  #endif
                  nop
                  nop
                  nop
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(0);
        }

        [TestMethod]
        public void NestedIfdefTrueTrueElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  #ifdef MySymbol2
                  nop ; 2
                  nop ; 3
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop ; 4
                  nop ; 5
                  nop ; 6
                  nop ; 7
                  #else
                  nop
                  nop
                  nop
                  nop
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(7);
        }

        [TestMethod]
        public void NestedIfdefTrueTrueElseNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  #ifdef MySymbol2
                  nop ; 2
                  nop ; 3
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop ; 4
                  nop ; 5
                  nop ; 6
                  nop ; 7
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(7);
        }

        [TestMethod]
        public void NestedIfdefTrueTrueNoElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  #ifdef MySymbol2
                  nop ; 2
                  nop ; 3
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop ; 4
                  nop ; 5
                  nop ; 6 
                  nop ; 7
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(7);
        }

        [TestMethod]
        public void NestedIfdefTrueFalseElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  #ifdef MySymbol2
                  nop
                  nop
                  #else
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5
                  nop ; 6
                  nop ; 7
                  nop ; 8
                  #else
                  nop
                  nop
                  nop
                  nop
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(8);
        }

        [TestMethod]
        public void NestedIfdefTrueFalseElseNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  #ifdef MySymbol2
                  nop
                  nop
                  #endif
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #else
                  nop
                  nop
                  nop
                  nop
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void NestedIfdefTrueFalseNoElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  #ifdef MySymbol2
                  nop
                  nop
                  #else
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5
                  nop ; 6
                  nop ; 7
                  nop ; 8
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(8);
        }

        [TestMethod]
        public void NestedIfdefFalseTrueElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void NestedIfdefFalseTrueNoElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop 
                  nop 
                  nop 
                  nop 
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(0);
        }

        [TestMethod]
        public void NestedIfdefFalseTrueElseNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #endif
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void NestedIfdefFalseFalseElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void NestedIfdefFalseFalseNoElseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop 
                  nop 
                  nop 
                  nop 
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(0);
        }

        [TestMethod]
        public void NestedIfdefFalseFalseElseNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #endif
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void ElseNestedIfdefTrueTrueElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #else
                  nop
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void ElseNestedIfdefTrueTrueNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #else
                  nop
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #endif
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void ElseNestedIfdefTrueFalseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #else
                  nop
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void ElseNestedIfdefTrueFalseNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop ; 1
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  nop ; 5
                  #else
                  nop
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #endif
                  nop 
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void ElseNestedIfdefFalseTrueElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  #ifdef MySymbol2
                  nop ; 2
                  nop ; 3
                  #else
                  nop
                  nop
                  nop
                  #endif
                  nop ; 4
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(4);
        }

        [TestMethod]
        public void ElseNestedIfdefFalseTrueNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL2");
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  #ifdef MySymbol2
                  nop ; 2
                  nop ; 3
                  #endif
                  nop ; 4
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(4);
        }

        [TestMethod]
        public void ElseNestedIfdefFalseFalseElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #else
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void ElseNestedIfdefFalseFalseNoElseWorks()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  #ifdef MySymbol
                  nop 
                  nop 
                  nop 
                  nop 
                  nop 
                  #else
                  nop ; 1
                  #ifdef MySymbol2
                  nop 
                  nop 
                  #endif
                  nop ; 2
                  #endif";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
        }

        [TestMethod]
        public void IfTrueWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  nop ; 1
                  #if 3 > 2
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfFalseWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  nop ; 1
                  #if 2 == 3
                  nop
                  nop
                  nop
                  #endif
                  nop ; 2";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
        }

        [TestMethod]
        public void IfTrueWorksWithElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            options.PredefinedSymbols.Add("MYSYMBOL");
            const string SOURCE = @"
                  nop ; 1
                  #if 6*8 != 49
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #else
                  nop
                  nop
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfFalseWorksWithElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions();
            const string SOURCE = @"
                  nop ; 1
                  #if 34 <= 13
                  nop
                  nop
                  nop
                  #else
                  nop ; 2
                  nop ; 3
                  #endif
                  nop ; 4";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(4);
        }

        [TestMethod]
        public void IfmodTrueWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions {CurrentModel = SpectrumModelType.Spectrum48};
            const string SOURCE = @"
                  nop ; 1
                  #ifmod Spectrum48
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfmodFalseWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions { CurrentModel = SpectrumModelType.Spectrum128 };
            const string SOURCE = @"
                  nop ; 1
                  #ifmod Spectrum48
                  nop
                  nop
                  nop
                  #endif
                  nop ; 2";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
        }

        [TestMethod]
        public void IfnmodTrueWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions { CurrentModel = SpectrumModelType.Spectrum48 };
            const string SOURCE = @"
                  nop ; 1
                  #ifnmod Spectrum48
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(2);
        }

        [TestMethod]
        public void IfnmodFalseWorksWithoutElseBranch()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions { CurrentModel = SpectrumModelType.Spectrum128 };
            const string SOURCE = @"
                  nop ; 1
                  #ifnmod Spectrum48
                  nop
                  nop
                  nop
                  #endif
                  nop ; 2";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(0);
            compiler.PreprocessedLines.Count.ShouldBe(5);
        }

        [TestMethod]
        public void IfmodWithInvalidModelTypeRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions { CurrentModel = SpectrumModelType.Spectrum48 };
            const string SOURCE = @"
                  nop ; 1
                  #ifmod Unknown
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0090);
        }

        [TestMethod]
        public void IfnmodWithInvalidModelTypeRaisesError()
        {
            // --- Arrange
            var compiler = new Z80Assembler();
            var options = new AssemblerOptions { CurrentModel = SpectrumModelType.Spectrum48 };
            const string SOURCE = @"
                  nop ; 1
                  #ifnmod Unknown
                  nop ; 2
                  nop ; 3
                  nop ; 4
                  #endif
                  nop ; 5";

            // --- Act
            var output = compiler.Compile(SOURCE, options);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0090);
        }
    }
}
