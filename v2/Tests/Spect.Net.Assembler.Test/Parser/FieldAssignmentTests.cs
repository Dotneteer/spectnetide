using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Pragmas;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class FieldAssignmentTests: ParserTestBed
    {
        [TestMethod]
        [DataRow("-> .defb 0x00")]
        [DataRow("field1 -> .defb 0x00")]
        [DataRow("field1: -> .defb 0x00")]
        public void DefbFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefbPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .defg ----OOOO")]
        [DataRow("field1 -> .defg OOOO----")]
        [DataRow("field1: -> .defg OOOO----")]
        public void DefgFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefgPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .defgx \"----OOOO\"")]
        [DataRow("field1 -> .defgx \"OOOO----\"")]
        [DataRow("field1: -> .defgx \"OOOO----\"")]
        public void DefgxFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefgxPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .defh \"12AC\"")]
        [DataRow("field1 -> .defh \"12AC\"")]
        [DataRow("field1: -> .defh \"12AC\"")]
        public void DefhFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefhPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .defc \"Hello\"")]
        [DataRow("field1 -> .defc \"Hello\"")]
        [DataRow("field1: -> .defc \"Hello\"")]
        [DataRow("-> .defm \"Hello\"")]
        [DataRow("field1 -> .defm \"Hello\"")]
        [DataRow("field1: -> .defm \"Hello\"")]
        [DataRow("-> .defn \"Hello\"")]
        [DataRow("field1 -> .defn \"Hello\"")]
        [DataRow("field1: -> .defn \"Hello\"")]
        public void DefmnFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefmnPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .defs 12")]
        [DataRow("field1 -> .defs 12")]
        [DataRow("field1: -> .defs 12")]
        public void DefsFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefsPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .defw 0x00")]
        [DataRow("field1 -> .defw 0x00")]
        [DataRow("field1: -> .defw 0x00")]
        public void DefwFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as DefwPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .fillb 0x01, 3")]
        [DataRow("field1 -> .fillb 0x01, 3")]
        [DataRow("field1: -> .fillb 0x01, 3")]
        public void FillbFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as FillbPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow("-> .fillw 0x01, 3")]
        [DataRow("field1 -> .fillw 0x01, 3")]
        [DataRow("field1: -> .fillw 0x01, 3")]
        public void FillwFieldAssignmentWorks(string source)
        {
            // --- Act
            var visitor = Parse(source);

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var assignment = visitor.Compilation.Lines[0] as FillwPragma;
            assignment.ShouldNotBeNull();
            assignment.IsFieldAssignment.ShouldBeTrue();
        }
    }
}
