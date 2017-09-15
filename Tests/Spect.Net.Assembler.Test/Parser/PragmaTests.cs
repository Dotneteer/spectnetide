using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Pragmas;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class PragmaTests : ParserTestBed
    {
        [TestMethod]
        public void OrgPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".org 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as OrgPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void OrgPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".ORG 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as OrgPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void OrgPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("org 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as OrgPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void OrgPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("origin ORG 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as OrgPragma;
            line.ShouldNotBeNull();
            line.Label.ShouldBe("ORIGIN");
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EntPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".ent 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EntPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EntPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".ENT 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EntPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EntPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("ent 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EntPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EntPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("entry ENT 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EntPragma;
            line.ShouldNotBeNull();
            line.Label.ShouldBe("ENTRY");
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DispPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".disp 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DispPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DispPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".DISP 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DispPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DispPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("disp 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DispPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DispPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("displacement DISP 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DispPragma;
            line.ShouldNotBeNull();
            line.Label.ShouldBe("DISPLACEMENT");
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EquPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".equ 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EquPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EquPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".EQU 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EquPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EquPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("equ 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EquPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void EquPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("equals EQU 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as EquPragma;
            line.ShouldNotBeNull();
            line.Label.ShouldBe("EQUALS");
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefwPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".defw #0100, #02aa");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefwPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefwPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".DEFW #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefwPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefwPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("defw #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefwPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefwPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("define DEFW #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefwPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
            line.Label.ShouldBe("DEFINE");
        }

        [TestMethod]
        public void DefbPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".defb #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefbPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefbPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".DEFB #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefbPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefbPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("defb #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefbPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefbPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("define DEFB #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefbPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
            line.Label.ShouldBe("DEFINE");
        }

        [TestMethod]
        public void DefmPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".defm \"Message with \\\" mark\"");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefmPragma;
            line.ShouldNotBeNull();
            line.Message.ShouldBe("\"Message with \\\" mark\"");
        }

        [TestMethod]
        public void SkipPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".skip 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as SkipPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void SkipPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".SKIP 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as SkipPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void SkipPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("skip 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as SkipPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void SkipPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("origin SKIP 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as SkipPragma;
            line.ShouldNotBeNull();
            line.Label.ShouldBe("ORIGIN");
            line.Expr.ShouldBeOfType<LiteralNode>();
        }


    }
}