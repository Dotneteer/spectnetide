using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Spect.Net.EvalParser.Test.Parser
{
    [TestClass]
    public class FormatSpecifierTest: ParserTestBed
    {
        [TestMethod]
        public void InvalidFormatSpecifierIsCaught()
        {
            var errors = ParseWithErrors("hl bla");
            errors.Count.ShouldBe(1);
        }

        [TestMethod]
        [DataRow(":b", "B")]
        [DataRow(":f", "F")]
        [DataRow(":F", "F")]
        [DataRow(":B", "B")]
        [DataRow(":-b", "-B")]
        [DataRow(":-B", "-B")]
        [DataRow(":c", "C")]
        [DataRow(":C", "C")]
        [DataRow(":h4", "H4")]
        [DataRow(":H4", "H4")]
        [DataRow(":h8", "H8")]
        [DataRow(":H8", "H8")]
        [DataRow(":w", "W")]
        [DataRow(":W", "W")]
        [DataRow(":-w", "-W")]
        [DataRow(":-W", "-W")]
        [DataRow(":dw", "DW")]
        [DataRow(":DW", "DW")]
        [DataRow(":-dw", "-DW")]
        [DataRow(":-DW", "-DW")]
        [DataRow(":%8", "%8")]
        [DataRow(":%16", "%16")]
        [DataRow(":%32", "%32")]
        public void FormatSpecifierParsingWorks(string spec, string expected)
        {
            var z80Expr = Parse("0 " + spec);
            z80Expr.FormatSpecifier.Format.ShouldBe(expected);
        }
    }
}
