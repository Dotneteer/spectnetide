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
        public void BackslashWorksAsExpected()
        {
            TestString(@"a\\b", 'a', '\\', 'b');
            TestString(@"\\b", '\\', 'b');
            TestString(@"a\\", 'a', '\\');
            TestString(@"ab\K", 'a', 'b', 'K');
            TestString(@"ab\", 'a', 'b', '\\');
        }

        [TestMethod]
        public void SingleQuoteWorksAsExpected()
        {
            TestString(@"a\'b", 'a', '\'', 'b');
            TestString(@"\'b", '\'', 'b');
            TestString(@"a\'", 'a', '\'');
        }

        [TestMethod]
        public void DoubleQuoteWorksAsExpected()
        {
            TestString("a\\\"b", 'a', '"', 'b');
            TestString("\\\"b", '"', 'b');
            TestString("a\\\"", 'a', '"');
        }

        [TestMethod]
        public void InkWorksAsExpected()
        {
            TestString(@"a\ib", 'a', 0x10, 'b');
            TestString(@"\ib", 0x10, 'b');
            TestString(@"a\i", 'a', 0x10);
        }

        [TestMethod]
        public void PaperWorksAsExpected()
        {
            TestString(@"a\pb", 'a', 0x11, 'b');
            TestString(@"\pb", 0x11, 'b');
            TestString(@"a\p", 'a', 0x11);
        }

        [TestMethod]
        public void FlashWorksAsExpected()
        {
            TestString(@"a\fb", 'a', 0x12, 'b');
            TestString(@"\fb", 0x12, 'b');
            TestString(@"a\f", 'a', 0x12);
        }

        [TestMethod]
        public void BrightWorksAsExpected()
        {
            TestString(@"a\bb", 'a', 0x13, 'b');
            TestString(@"\bb", 0x13, 'b');
            TestString(@"a\b", 'a', 0x13);
        }

        [TestMethod]
        public void InverseWorksAsExpected()
        {
            TestString(@"a\Ib", 'a', 0x14, 'b');
            TestString(@"\Ib", 0x14, 'b');
            TestString(@"a\I", 'a', 0x14);
        }

        [TestMethod]
        public void OverWorksAsExpected()
        {
            TestString(@"a\ob", 'a', 0x15, 'b');
            TestString(@"\ob", 0x15, 'b');
            TestString(@"a\o", 'a', 0x15);
        }

        [TestMethod]
        public void AtWorksAsExpected()
        {
            TestString(@"a\ab", 'a', 0x16, 'b');
            TestString(@"\ab", 0x16, 'b');
            TestString(@"a\a", 'a', 0x16);
        }

        [TestMethod]
        public void TabWorksAsExpected()
        {
            TestString(@"a\tb", 'a', 0x17, 'b');
            TestString(@"\tb", 0x17, 'b');
            TestString(@"a\t", 'a', 0x17);
        }

        [TestMethod]
        public void PoundSignWorksAsExpected()
        {
            TestString(@"a\Pb", 'a', 0x60, 'b');
            TestString(@"\Pb", 0x60, 'b');
            TestString(@"a\P", 'a', 0x60);
        }

        [TestMethod]
        public void CopyrightSignWorksAsExpected()
        {
            TestString(@"a\Cb", 'a', 0x7F, 'b');
            TestString(@"\Cb", 0x7F, 'b');
            TestString(@"a\C", 'a', 0x7F);
        }

        [TestMethod]
        public void BinZeroWorksAsExpected()
        {
            TestString(@"a\0b", 'a', 0x00, 'b');
            TestString(@"\0b", 0x00, 'b');
            TestString(@"a\0", 'a', 0x00);
        }

        [TestMethod]
        public void SingleHexEscapeWorksAsExpected()
        {
            TestString(@"a\x1Q", 'a', 0x01, 'Q');
            TestString(@"\xaQ", 0x0A, 'Q');
            TestString(@"\xCQ", 0x0C, 'Q');
            TestString(@"a\x1", 'a', 0x01);
        }

        [TestMethod]
        public void DoubleHexEscapeWorksAsExpected()
        {
            TestString(@"a\x1cQ", 'a', 0x1C, 'Q');
            TestString(@"\xa3Q", 0xA3, 'Q');
            TestString(@"\xCBQ", 0xCB, 'Q');
        }

        [TestMethod]
        public void ExtraHexEscapeWorksAsExpected()
        {
            TestString(@"a\x1c4Q", 'a', 0x1C, '4', 'Q');
            TestString(@"\xa35Q", 0xA3, '5', 'Q');
            TestString(@"\xCBAQ", 0xCB, 'A', 'Q');
        }

        [TestMethod]
        public void IncompleteHexEscapeWorksAsExpected()
        {
            TestString(@"a0xQ", 'a', '0', 'x', 'Q');
            TestString(@"0xQ", '0', 'x', 'Q');
            TestString(@"0x", '0', 'x');
            TestString(@"a0k", 'a', '0', 'k');
            TestString(@"a0k\i", 'a', '0', 'k', 0x10);
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
                else if (bytes[i] is int bt)
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
