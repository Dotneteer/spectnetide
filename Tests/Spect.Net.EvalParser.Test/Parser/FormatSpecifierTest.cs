using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Spect.Net.EvalParser.Test.Parser
{
    [TestClass]
    public class FormatSpecifierTest: ParserTestBed
    {
        [TestMethod]
        [DataRow(":b", "B")]
        [DataRow(":B", "B")]
        [DataRow(":-b", "-B")]
        [DataRow(":-B", "-B")]
        [DataRow(":c", "C")]
        [DataRow(":C", "C")]
        [DataRow(":h2", "H2")]
        [DataRow(":H2", "H2")]
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
        [DataRow(":bv8", "BV8")]
        [DataRow(":BV8", "BV8")]
        [DataRow(":bv16", "BV16")]
        [DataRow(":BV16", "BV16")]
        [DataRow(":s", "S")]
        [DataRow(":S", "S")]
        [DataRow(":s0", "S0")]
        [DataRow(":S0", "S0")]
        public void FormatSpecifierParsingWorks(string spec, string expected)
        {
            var z80Expr = Parse("0 " + spec);
            z80Expr.FormatSpecifier.Format.ShouldBe(expected);
        }
    }
}
