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
        public void XentPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".xent 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as XentPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void XentPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".XENT 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as XentPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void XentPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("xent 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as XentPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void XentPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("entry XENT 8000H");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as XentPragma;
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
        [DataRow("dw")]
        [DataRow(".dw")]
        [DataRow("DW")]
        [DataRow(".DW")]
        public void DefwPragmaWorksAsExpected5(string pragma)
        {
            // --- Act
            var visitor = Parse($"{pragma} #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefwPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
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
        [DataRow("db")]
        [DataRow(".db")]
        [DataRow("DB")]
        [DataRow(".DB")]
        public void DefbPragmaWorksAsExpected5(string pragma)
        {
            // --- Act
            var visitor = Parse($"{pragma} #01, #02");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefbPragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(2);
            line.Exprs[0].ShouldBeOfType<LiteralNode>();
            line.Exprs[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow("defm")]
        [DataRow("DEFM")]
        [DataRow("dm")]
        [DataRow("DM")]
        [DataRow(".defm")]
        [DataRow(".DEFM")]
        [DataRow(".dm")]
        [DataRow(".DM")]
        public void DefmPragmaWorksAsExpected(string pragma)
        {
            // --- Act
            var visitor = Parse($"{pragma} \"Message with \\\" mark\"");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefmnPragma;
            line.ShouldNotBeNull();
            line.Message.ShouldNotBeNull();
            line.NullTerminator.ShouldBeFalse();
        }

        [TestMethod]
        [DataRow("defn")]
        [DataRow("DEFN")]
        [DataRow("dn")]
        [DataRow("DN")]
        [DataRow(".defn")]
        [DataRow(".DEFN")]
        [DataRow(".dn")]
        [DataRow(".DN")]
        public void DefnPragmaWorksAsExpected(string pragma)
        {
            // --- Act
            var visitor = Parse($"{pragma} \"Message with \\\" mark\"");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefmnPragma;
            line.ShouldNotBeNull();
            line.Message.ShouldNotBeNull();
            line.NullTerminator.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("defh")]
        [DataRow("DEFH")]
        [DataRow("dh")]
        [DataRow("DH")]
        [DataRow(".defh")]
        [DataRow(".DEFH")]
        [DataRow(".dh")]
        [DataRow(".DH")]
        public void DefhPragmaWorksAsExpected(string pragma)
        {
            // --- Act
            var visitor = Parse($"{pragma} \"Message with \\\" mark\"");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefhPragma;
            line.ShouldNotBeNull();
            line.ByteVector.ShouldNotBeNull();
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

        [TestMethod]
        public void DefsPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".defs 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".DEFS 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("defs 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("DEFS 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected5()
        {
            // --- Act
            var visitor = Parse(".ds 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected6()
        {
            // --- Act
            var visitor = Parse(".DS 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected7()
        {
            // --- Act
            var visitor = Parse("ds 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DefsPragmaWorksAsExpected8()
        {
            // --- Act
            var visitor = Parse("DS 22");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefsPragma;
            line.ShouldNotBeNull();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillbPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".fillb 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillbPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillbPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".FILLB 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillbPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillbPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("fillb 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillbPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillbPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("FILLB 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillbPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillwPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".fillw 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillwPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillwPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".FILLW 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillwPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillwPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("fillw 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillwPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void FillwPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("FILLW 10,15");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as FillwPragma;
            line.ShouldNotBeNull();
            line.Count.ShouldBeOfType<LiteralNode>();
            line.Expression.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void ModelPragmaWorksAsExpected1()
        {
            // --- Act
            var visitor = Parse(".model spectrum48");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModelPragma;
            line.ShouldNotBeNull();
            line.Model.ShouldBe("SPECTRUM48");
        }

        [TestMethod]
        public void ModelPragmaWorksAsExpected2()
        {
            // --- Act
            var visitor = Parse(".MODEL SPECTRUM128");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModelPragma;
            line.ShouldNotBeNull();
            line.Model.ShouldBe("SPECTRUM128");
        }

        [TestMethod]
        public void ModelPragmaWorksAsExpected3()
        {
            // --- Act
            var visitor = Parse("model SPECTRUMP3");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModelPragma;
            line.ShouldNotBeNull();
            line.Model.ShouldBe("SPECTRUMP3");
        }

        [TestMethod]
        public void ModelPragmaWorksAsExpected4()
        {
            // --- Act
            var visitor = Parse("model spectrump3");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModelPragma;
            line.ShouldNotBeNull();
            line.Model.ShouldBe("SPECTRUMP3");
        }

        [TestMethod]
        public void ModelPragmaWorksAsExpected5()
        {
            // --- Act
            var visitor = Parse("MODEL next");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModelPragma;
            line.ShouldNotBeNull();
            line.Model.ShouldBe("NEXT");
        }

        [TestMethod]
        public void ModelPragmaWorksAsExpected6()
        {
            // --- Act
            var visitor = Parse("MODEL NEXT");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ModelPragma;
            line.ShouldNotBeNull();
            line.Model.ShouldBe("NEXT");
        }

        [TestMethod]
        [DataRow(".align")]
        [DataRow("align")]
        [DataRow(".ALIGN")]
        [DataRow("ALIGN")]
        public void AlignPragmaWorksWithoutExpression(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as AlignPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeNull();
        }

        [TestMethod]
        [DataRow(".align #100")]
        [DataRow("align #100")]
        [DataRow(".ALIGN #100")]
        [DataRow("ALIGN #100")]
        public void AlignPragmaWorksWithExpression(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as AlignPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow("trace #100", 1, false)]
        [DataRow(".trace #100", 1, false)]
        [DataRow("TRACE #100", 1, false)]
        [DataRow(".TRACE #100", 1, false)]
        [DataRow("tracehex #100", 1, true)]
        [DataRow(".tracehex #100", 1, true)]
        [DataRow("TRACEHEX #100", 1, true)]
        [DataRow(".TRACEHEX #100", 1, true)]
        [DataRow("trace #100, #200, \"Hello\"", 3, false)]
        public void TracePragmaWorksWith(string source, int exprCount, bool isHex)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as TracePragma;
            line.ShouldNotBeNull();
            line.Exprs.Count.ShouldBe(exprCount);
            line.IsHex.ShouldBe(isHex);
        }

        [TestMethod]
        [DataRow(".rndseed")]
        [DataRow("rndseed")]
        [DataRow(".RNDSEED")]
        [DataRow("RNDSEED")]
        public void RndSeedPragmaWorksWithoutExpression(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as RndSeedPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeNull();
        }

        [TestMethod]
        [DataRow(".rndseed #100")]
        [DataRow("rndseed #100")]
        [DataRow(".RNDSEED #100")]
        [DataRow("RNDSEED #100")]
        public void RndSeedPragmaWorksWithExpression(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as RndSeedPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow(".defg")]
        [DataRow("defg")]
        [DataRow(".DEFG")]
        [DataRow("DEFG")]
        [DataRow(".dg")]
        [DataRow("dg")]
        [DataRow(".DG")]
        [DataRow("DG")]
        public void DefgPragmaWorksAsExpected(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefgPragma;
            line.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow(".defgx \"___O\"")]
        [DataRow("defgx \"___O\"")]
        [DataRow(".DEFGX \"___O\"")]
        [DataRow("DEFGX \"___O\"")]
        [DataRow(".dgx \"___O\"")]
        [DataRow("dgx \"___O\"")]
        [DataRow(".DGX \"___O\"")]
        [DataRow("DGX \"___O\"")]
        public void DefgxPragmaWorksAsExpected(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as DefgxPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow(".error \"message\"")]
        [DataRow(".ERROR \"message\"")]
        [DataRow("error \"message\"")]
        [DataRow("ERROR \"message\"")]
        public void ErrorPragmaWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as ErrorPragma;
            line.ShouldNotBeNull();
            line.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        [DataRow(".includebin \"file\"")]
        [DataRow(".INCLUDEBIN \"file\"")]
        [DataRow("includebin \"file\"")]
        [DataRow("INCLUDEBIN \"file\"")]
        [DataRow(".include_bin \"file\"")]
        [DataRow(".INCLUDE_BIN \"file\"")]
        [DataRow("include_bin \"file\"")]
        [DataRow("INCLUDE_BIN \"file\"")]
        public void IncBinPragmaWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IncludeBinPragma;
            line.ShouldNotBeNull();
            line.FileExpr.ShouldBeOfType<LiteralNode>();
            line.OffsetExpr.ShouldBeNull();
            line.LengthExpr.ShouldBeNull();
        }

        [TestMethod]
        [DataRow(".includebin \"file\", 100")]
        [DataRow(".INCLUDEBIN \"file\", 100")]
        [DataRow("includebin \"file\", 100")]
        [DataRow("INCLUDEBIN \"file\", 100")]
        [DataRow(".include_bin \"file\", 100")]
        [DataRow(".INCLUDE_BIN \"file\", 100")]
        [DataRow("include_bin \"file\", 100")]
        [DataRow("INCLUDE_BIN \"file\", 100")]
        public void IncBinPragmaWithOffsetWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IncludeBinPragma;
            line.ShouldNotBeNull();
            line.FileExpr.ShouldBeOfType<LiteralNode>();
            line.OffsetExpr.ShouldBeOfType<LiteralNode>();
            line.LengthExpr.ShouldBeNull();
        }

        [TestMethod]
        [DataRow(".includebin \"file\", 100, 200")]
        [DataRow(".INCLUDEBIN \"file\", 100, 200")]
        [DataRow("includebin \"file\", 100, 200")]
        [DataRow("INCLUDEBIN \"file\", 100, 200")]
        [DataRow(".include_bin \"file\", 100, 200")]
        [DataRow(".INCLUDE_BIN \"file\", 100, 200")]
        [DataRow("include_bin \"file\", 100, 200")]
        [DataRow("INCLUDE_BIN \"file\", 100, 200")]
        public void IncBinPragmaWithOffsetAndLengthWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0] as IncludeBinPragma;
            line.ShouldNotBeNull();
            line.FileExpr.ShouldBeOfType<LiteralNode>();
            line.OffsetExpr.ShouldBeOfType<LiteralNode>();
            line.LengthExpr.ShouldBeOfType<LiteralNode>();
        }

    }
}