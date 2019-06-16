using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.ZxBasic;
// ReSharper disable IdentifierTypo

namespace Spect.Net.VsPackage.Test.ZxBasic
{
    [TestClass]
    public class ZxbOptionsTest
    {
        [TestMethod]
        public void NoOptionsWorks()
        {
            // --- Act
            var opt = CreateOptions();

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + " --optimize 1 --org 32768 --heap-size 4096");
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 3)]
        [DataRow(4, 3)]
        public void OptimizeOptionsWorks(int optimize, int expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.Optimize = (ushort)optimize;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize {expected} --org 32768 --heap-size 4096");
        }

        [TestMethod]
        [DataRow(0x6000)]
        [DataRow(0x7000)]
        [DataRow(0x8000)]
        [DataRow(0xA000)]
        [DataRow(0xC000)]
        public void OrgOptionsWorks(int org)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.OrgValue = (ushort)org;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org {org} --heap-size 4096");
        }

        [TestMethod]
        [DataRow(0x1000)]
        [DataRow(0x2000)]
        [DataRow(0x3000)]
        [DataRow(0x0800)]
        [DataRow(0x0400)]
        public void HeapSizeOptionsWorks(int heapSize)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.HeapSize = (ushort)heapSize;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size {heapSize}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --tzx")]
        [DataRow(false, "")]
        public void TzxFormatWorks(bool? tzx, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.TzxFormat = tzx;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}" );
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --tap")]
        [DataRow(false, "")]
        public void TapFormatWorks(bool? tap, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.TapFormat = tap;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --BASIC")]
        [DataRow(false, "")]
        public void BasicLoaderWorks(bool? basic, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.BasicLoader = basic;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --autorun")]
        [DataRow(false, "")]
        public void AutoRunWorks(bool? autorun, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.AutoRun = autorun;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --asm")]
        [DataRow(false, "")]
        public void AsmFormatWorks(bool? asm, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.AsmFormat = asm;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --array-base")]
        [DataRow(false, "")]
        public void ArrayBaseOneWorks(bool? arraybase, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.ArrayBaseOne = arraybase;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --string-base")]
        [DataRow(false, "")]
        public void StringBaseOneWorks(bool? stringbase, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.StringBaseOne = stringbase;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --sinclair")]
        [DataRow(false, "")]
        public void SinclairFlagOneWorks(bool? sinclair, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.SinclairFlag = sinclair;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --debug-memory")]
        [DataRow(false, "")]
        public void DebugMemoryWorks(bool? debugmem, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.DebugMemory = debugmem;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --debug-array")]
        [DataRow(false, "")]
        public void DebugArrayWorks(bool? debugarr, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.DebugArray = debugarr;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --strict-bool")]
        [DataRow(false, "")]
        public void StrictBoolWorks(bool? strict, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.StrictBool = strict;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --enable-break")]
        [DataRow(false, "")]
        public void EnableBreakWorks(bool? enable, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.EnableBreak = enable;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --explicit")]
        [DataRow(false, "")]
        public void ExplicitDimWorks(bool? explicitDim, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.ExplicitDim = explicitDim;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        [TestMethod]
        [DataRow(null, "")]
        [DataRow(true, " --strict")]
        [DataRow(false, "")]
        public void StrictTypesWorks(bool? strict, string expected)
        {
            // --- Arrange
            var opt = CreateOptions();

            // --- Act
            opt.StrictTypes = strict;

            // --- Assert
            opt.ToString().ShouldBe(BaseOptionString(opt) + $" --optimize 1 --org 32768 --heap-size 4096{expected}");
        }

        private ZxbOptions CreateOptions()
        {
            return new ZxbOptions
            {
                ProgramFilename = "program.bas",
                OutputFilename = "program.bin",
                ErrorFilename = "error.txt"
            };
        }

        private string BaseOptionString(ZxbOptions options) =>
            $"{options.ProgramFilename} --output {options.OutputFilename} --stderr {options.ErrorFilename}";
    }
}
