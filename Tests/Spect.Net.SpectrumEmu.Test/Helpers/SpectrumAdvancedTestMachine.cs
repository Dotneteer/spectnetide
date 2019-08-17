using System.Collections.Generic;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class SpectrumAdvancedTestMachine: SpectrumEngine, IStackDebugSupport
    {
        public List<StackPointerManipulationEvent> StackPointerManipulations { get; }

        public List<StackContentManipulationEvent> StackContentManipulations { get; }

        public List<BranchEvent> BranchEvents { get; }

        private readonly Stack<ushort> _stepOutStack = new Stack<ushort>();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SpectrumAdvancedTestMachine(IScreenFrameProvider renderer = null,
            IScreenConfiguration screenConfig = null, ICpuConfiguration cpuConfig = null, string ulaIssue = "3") :
            base(new DeviceInfoCollection
            {
                new CpuDeviceInfo(cpuConfig ?? SpectrumModels.ZxSpectrum48Pal.Cpu),
                new RomDeviceInfo(new ResourceRomProvider(typeof(RomResourcesPlaceHolder).Assembly),
                    new RomConfigurationData
                    {
                        NumberOfRoms = 1,
                        RomName = "ZxSpectrum48",
                        Spectrum48RomIndex = 0
                    },
                    new SpectrumRomDevice()),
                new MemoryDeviceInfo(
                    new MemoryConfigurationData
                    {
                        SupportsBanking = false,
                        ContentionType = MemoryContentionType.Ula
                    }, null),
                new ClockDeviceInfo(new ClockProvider()),
                new BeeperDeviceInfo(new AudioConfigurationData
                {
                    AudioSampleRate = 35000,
                    SamplesPerFrame = 699,
                    TactsPerSample = 100
                }, null),
                new ScreenDeviceInfo(screenConfig ?? SpectrumModels.ZxSpectrum48Pal.Screen,
                    renderer ?? new TestPixelRenderer(screenConfig ?? SpectrumModels.ZxSpectrum48Pal.Screen)),
                new KempstonDeviceInfo(new KempstonTestProvider())
            }, ulaIssue)
        {
            StackPointerManipulations = new List<StackPointerManipulationEvent>();
            StackContentManipulations = new List<StackContentManipulationEvent>();
            BranchEvents = new List<BranchEvent>();
            Cpu.StackDebugSupport = this;
        }

        /// <summary>
        /// Initializes the code passed in <paramref name="programCode"/>. This code
        /// is put into the memory from <paramref name="codeAddress"/> and
        /// </summary>
        /// <param name="programCode">Program code</param>
        /// <param name="codeAddress">Address of first code byte</param>
        /// <param name="startAddress">Code start address, null if same as the first byte</param>
        public void InitCode(IEnumerable<byte> programCode = null, ushort codeAddress = 0x8000, 
            ushort? startAddress = null)
        {
            if (programCode == null) return;
            if (startAddress == null) startAddress = codeAddress;

            // --- Initialize the code
            foreach (var op in programCode)
            {
                WriteSpectrumMemory(codeAddress++, op);
            }
            while (codeAddress != 0)
            {
                WriteSpectrumMemory(codeAddress++, 0);
            }

            Cpu.Reset();
            Cpu.Registers.PC = startAddress.Value;
        }

        /// <summary>
        /// Resets the debug support
        /// </summary>
        void IStackDebugSupport.Reset()
        {
        }

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev)
        {
            StackPointerManipulations.Add(ev);
        }

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackContentManipulationEvent(StackContentManipulationEvent ev)
        {
            StackContentManipulations.Add(ev);
        }

        /// <summary>
        /// Checks if the Step-Out stack contains any information
        /// </summary>
        /// <returns></returns>
        public bool HasStepOutInfo()
        {
            return _stepOutStack.Count > 0;
        }

        /// <summary>
        /// The depth of the Step-Out stack
        /// </summary>
        public int StepOutStackDepth => _stepOutStack.Count;

        /// <summary>
        /// Clears the content of the Step-Out stack
        /// </summary>
        public void ClearStepOutStack()
        {
            _stepOutStack.Clear();
        }

        /// <summary>
        /// Pushes the specified return address to the Step-Out stack
        /// </summary>
        /// <param name="address"></param>
        public void PushStepOutAddress(ushort address)
        {
            _stepOutStack.Push(address);
        }

        /// <summary>
        /// Pops a Step-Out return point address from the stack
        /// </summary>
        /// <returns>Address popped from the stack</returns>
        /// <returns>Zeor, if the Step-Out stack is empty</returns>
        public ushort PopStepOutAddress()
        {
            if (_stepOutStack.Count > 0)
            {
                StepOutAddress = _stepOutStack.Pop();
                return StepOutAddress.Value;
            }
            StepOutAddress = null;
            return 0;
        }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a CALL
        /// </summary>
        public bool CallExecuted { get; set; }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a RET
        /// </summary>
        public bool RetExecuted { get; set; }

        /// <summary>
        /// Gets the last popped Step-Out address
        /// </summary>
        public ushort? StepOutAddress { get; set; }

        /// <summary>
        /// Records a branching event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordBranchEvent(BranchEvent ev)
        {
            BranchEvents.Add(ev);
        }
    }
}