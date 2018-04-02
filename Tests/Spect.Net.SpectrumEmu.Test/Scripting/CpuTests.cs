using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Scripting
{
    [TestClass]
    public class CpuTests
    {
        private const string STATE_FOLDER = @"C:\Temp\SavedState";

        [TestMethod]
        public async Task InterruptExecutingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var counter = 0;

            // --- Act
            sm.Cpu.InterruptExecuting += (s, e) => { counter++; };
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld a,2
                out (#fe),a
                halt
                halt
                halt
                ret
            ");
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            counter.ShouldBe(4);
        }

        [TestMethod]
        public async Task MemoryReadingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld hl,#4000
                ld a,(hl)
                ld a,(#1234)
                ret
            ");
            sm.Cpu.MemoryReading += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2 - 2; // -2 for RET, -2 for the two memory reads
            events[sampleIndex].Address.ShouldBe((ushort)0x4000);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x1234);
        }

        [TestMethod]
        public async Task MemoryReadIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressAndDataEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld hl,#4000
                ld (hl),#AA
                ld a,(hl)
                inc hl
                ld (hl),#55
                ld a,(#4001)
                ret
            ");
            sm.Cpu.MemoryRead += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2 - 2; // -2 for RET, -2 for the two memory reads
            events[sampleIndex].Address.ShouldBe((ushort)0x4000);
            events[sampleIndex].Data.ShouldBe((byte)0xAA);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x4001);
            events[sampleIndex + 1].Data.ShouldBe((byte)0x55);
        }

        [TestMethod]
        public async Task MemoryWritingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressAndDataEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld hl,#4000
                ld (hl),#AA
                ld a,(hl)
                inc hl
                ld (hl),#55
                ld a,(#4001)
                ret
            ");
            sm.Cpu.MemoryWriting += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2; // -2 for the two memory writes
            events[sampleIndex].Address.ShouldBe((ushort)0x4000);
            events[sampleIndex].Data.ShouldBe((byte)0xAA);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x4001);
            events[sampleIndex + 1].Data.ShouldBe((byte)0x55);
        }

        [TestMethod]
        public async Task MemoryWrittenIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressAndDataEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld hl,#4000
                ld (hl),#AA
                ld a,(hl)
                inc hl
                ld (hl),#55
                ld a,(#4001)
                ret
            ");
            sm.Cpu.MemoryWritten += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2; // -2 for the two memory writes
            events[sampleIndex].Address.ShouldBe((ushort)0x4000);
            events[sampleIndex].Data.ShouldBe((byte)0xAA);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x4001);
            events[sampleIndex + 1].Data.ShouldBe((byte)0x55);
        }

        [TestMethod]
        public async Task PortReadingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld bc,#7FFE
                in a,(c)
                ld a,#24
                in a,(#FE)
                ret
            ");
            sm.Cpu.PortReading += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2; // -2 for the 2 read operation
            events[sampleIndex].Address.ShouldBe((ushort)0x7FFE);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x24FE);
        }

        [TestMethod]
        public async Task PortReadIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressAndDataEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld bc,#7FFE
                in a,(c)
                ld a,#24
                in a,(#FE)
                ret
            ");
            sm.Cpu.PortRead += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2; // -2 for the 2 read operation
            events[sampleIndex].Address.ShouldBe((ushort)0x7FFE);
            events[sampleIndex].Data.ShouldBe((byte)0xFF);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x24FE);
            events[sampleIndex + 1].Data.ShouldBe((byte)0xFF);
        }

        [TestMethod]
        public async Task PortWritingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressAndDataEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld a,5
                ld bc,#7FFE
                out (c),a
                ld a,#24
                out (#FE),a
                ret
            ");
            sm.Cpu.PortWriting += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2; // -2 for the 2 read operation
            events[sampleIndex].Address.ShouldBe((ushort)0x7FFE);
            events[sampleIndex].Data.ShouldBe((byte)0x05);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x24FE);
            events[sampleIndex + 1].Data.ShouldBe((byte)0x24);
        }

        [TestMethod]
        public async Task PortWrittenIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<AddressAndDataEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                ld a,5
                ld bc,#7FFE
                out (c),a
                ld a,#24
                out (#FE),a
                ret
            ");
            sm.Cpu.PortWritten += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 2; // -2 for the 2 read operation
            events[sampleIndex].Address.ShouldBe((ushort)0x7FFE);
            events[sampleIndex].Data.ShouldBe((byte)0x05);
            events[sampleIndex + 1].Address.ShouldBe((ushort)0x24FE);
            events[sampleIndex + 1].Data.ShouldBe((byte)0x24);
        }

        [TestMethod]
        public async Task OperationExecutingIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<Z80InstructionExecutionEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                di;              8000: No prefix
                bit 3,a;         8001: CB prefix
                ld a,(ix+2);     8003: DD prefix
                ld a,(iy+6);     8006: FD prefix
                bit 2,(ix+2);    8009: DD CB prefixes
                bit 2,(iy+6);    800D: FD CB prefixes
                in d,(c);        8011: ED prefix
                ret
            ");
            sm.Cpu.OperationExecuting += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 1 - 7; // -1 for the RET operation, -7 for the 7 operations
            var op = events[sampleIndex];
            op.PcBefore.ShouldBe((ushort)0x8000);
            op.Instruction.SequenceEqual(new byte[] { 0xF3 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0xF3);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 1];
            op.PcBefore.ShouldBe((ushort)0x8001);
            op.Instruction.SequenceEqual(new byte []{ 0xCB, 0x5F}).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x5F);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 2];
            op.PcBefore.ShouldBe((ushort)0x8003);
            op.Instruction.SequenceEqual(new byte[] { 0xDD, 0x7E }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x7E);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 3];
            op.PcBefore.ShouldBe((ushort)0x8006);
            op.Instruction.SequenceEqual(new byte[] { 0xFD, 0x7E }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x7E);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 4];
            op.PcBefore.ShouldBe((ushort)0x8009);
            op.Instruction.SequenceEqual(new byte[] { 0xDD, 0xCB, 0x56 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x56);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 5];
            op.PcBefore.ShouldBe((ushort)0x800D);
            op.Instruction.SequenceEqual(new byte[] { 0xFD, 0xCB, 0x56 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x56);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 6];
            op.PcBefore.ShouldBe((ushort)0x8011);
            op.Instruction.SequenceEqual(new byte[] { 0xED, 0x50 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x50);
            op.PcAfter.ShouldBeNull();
        }

        [TestMethod]
        public async Task OperationExecutedIsInvoked()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.CachedVmStateFolder = STATE_FOLDER;
            await sm.StartAndRunToMain(true);
            var events = new List<Z80InstructionExecutionEventArgs>();

            // --- Act
            var entryAddress = sm.InjectCode(@"
                .org #8000
                di;              8000: No prefix
                bit 3,a;         8001: CB prefix
                ld a,(ix+2);     8003: DD prefix
                ld a,(iy+6);     8006: FD prefix
                bit 2,(ix+2);    8009: DD CB prefixes
                bit 2,(iy+6);    800D: FD CB prefixes
                in d,(c);        8011: ED prefix
                ret
            ");
            sm.Cpu.OperationExecuted += (s, e) => { events.Add(e); };
            sm.CallCode(entryAddress);
            await sm.CompletionTask;

            // --- Assert
            var sampleIndex = events.Count - 1 - 7; // -1 for the RET operation, -7 for the 7 operations
            var op = events[sampleIndex];
            op.PcBefore.ShouldBe((ushort)0x8000);
            op.Instruction.SequenceEqual(new byte[] { 0xF3 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0xF3);
            op.PcAfter.ShouldBe((ushort)0x8001);

            op = events[sampleIndex + 1];
            op.PcBefore.ShouldBe((ushort)0x8001);
            op.Instruction.SequenceEqual(new byte[] { 0xCB, 0x5F }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x5F);
            op.PcAfter.ShouldBe((ushort)0x8003);

            op = events[sampleIndex + 2];
            op.PcBefore.ShouldBe((ushort)0x8003);
            op.Instruction.SequenceEqual(new byte[] { 0xDD, 0x7E, 0x02}).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x7E);
            op.PcAfter.ShouldBe((ushort)0x8006);

            op = events[sampleIndex + 3];
            op.PcBefore.ShouldBe((ushort)0x8006);
            op.Instruction.SequenceEqual(new byte[] { 0xFD, 0x7E }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x7E);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 4];
            op.PcBefore.ShouldBe((ushort)0x8009);
            op.Instruction.SequenceEqual(new byte[] { 0xDD, 0xCB, 0x56 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x56);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 5];
            op.PcBefore.ShouldBe((ushort)0x800D);
            op.Instruction.SequenceEqual(new byte[] { 0xFD, 0xCB, 0x56 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x56);
            op.PcAfter.ShouldBeNull();

            op = events[sampleIndex + 6];
            op.PcBefore.ShouldBe((ushort)0x8011);
            op.Instruction.SequenceEqual(new byte[] { 0xED, 0x50 }).ShouldBeTrue();
            op.OpCode.ShouldBe((byte)0x50);
            op.PcAfter.ShouldBeNull();
        }

    }
}
