using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class SpectrumStringTests
    {
        [TestMethod]
        public void NormalCharsWorkAsExpected()
        {
            TestString("abcd1234,", 'a', 'b', 'c', 'd', '1', '2', '3', '4', ',');
        }

        [TestMethod]
        public void BackslashWorkAsExpected()
        {
            TestString(@"a\\b", 'a', 'b', 'c', 'd', '1', '2', '3', '4', ',');
        }

        private void TestString(string input, params object[] bytes)
        {
            var result = Z80Assembler.SpectrumStringToBytes(input);
            result.Count.ShouldBe(bytes.Length);
            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] is char ch)
                {
                    ((byte) ch).ShouldBe(result[i]);
                }
                else if (bytes[i] is byte bt)
                {
                    bt.ShouldBe(result[i]);
                }
                else
                {
                    Assert.Fail("Byte or char input expected");
                }
            }
        }
    }
}
