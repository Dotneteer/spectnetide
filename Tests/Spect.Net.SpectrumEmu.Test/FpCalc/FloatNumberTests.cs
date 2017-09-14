using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.FpCalc;

namespace Spect.Net.SpectrumEmu.Test.FpCalc
{
    [TestClass]
    public class FloatNumberTests
    {
        [TestMethod]
        public void FromBytesWorksAsExpected()
        {
            FloatNumber.FromBytes(new List<byte>{0x00, 0xFF, 0x0A, 0x00, 0x00}).ShouldBe(-10.0f);
            FloatNumber.FromBytes(new List<byte> { 0x81, 0x49, 0x0F, 0xDA, 0xA2 }).ShouldBe((float)Math.PI/2);
        }

        [TestMethod]
        public void FromCompactedBytesWorksAsExpected()
        {
            TestFloat(new List<byte> { 0x00, 0xB0, 0x00 }, 0.0f);
            TestFloat(new List<byte> { 0x40, 0xB0, 0x00, 0x01 }, 1.0f);
            TestFloat(new List<byte> { 0x30, 0x00 }, 0.5f);
            TestFloat(new List<byte> { 0xF1, 0x49, 0x0F, 0xDA, 0xA2 }, (float)Math.PI / 2);
            TestFloat(new List<byte> { 0x40, 0xB0, 0x00, 0x0A }, 10.0f);
            TestFloat(new List<byte> { 0x14, 0xE6 }, -3.346941E-09f);
            TestFloat(new List<byte> { 0x5C, 0x1F, 0x0B }, 5.924812E-07f);
            TestFloat(new List<byte> { 0xA3, 0x8F, 0x38, 0xEE }, -6.829375E-05f);
            TestFloat(new List<byte> { 0xE9, 0x15, 0x63, 0xBB, 0x23 }, 0.004559008f);
            TestFloat(new List<byte> { 0xEE, 0x92, 0x0D, 0xCD, 0xED }, -0.142630786f);
            TestFloat(new List<byte> { 0xF1, 0x23, 0x5D, 0x1B, 0xEA }, 1.276279f);
        }

        private void TestFloat(List<byte> bytes, float value)
        {
            var convVal = FloatNumber.FromCompactBytes(bytes);
            Math.Abs(convVal - value).ShouldBeLessThanOrEqualTo((float)1.0e-12);
        }
    }
}
