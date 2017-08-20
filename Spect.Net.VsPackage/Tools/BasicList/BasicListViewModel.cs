using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Mvvm.Messages;

namespace Spect.Net.VsPackage.Tools.BasicList
{
    /// <summary>
    /// This view model represent the ZX Spectrum BASIC List
    /// </summary>
    public class BasicListViewModel: SpectrumGenericToolWindowViewModel
    {
        private byte[] _memory;
        private bool _tokensRead;
        private List<string> _tokens;

        /// <summary>
        /// Program lines decoded
        /// </summary>
        public ObservableCollection<BasicLineViewModel> ProgramLines { get; } = new ObservableCollection<BasicLineViewModel>();

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public BasicListViewModel()
        {
            if (!IsInDesignMode) return;

            ProgramLines = new ObservableCollection<BasicLineViewModel>
            {
                new BasicLineViewModel {LineNo = 10, Length = 10, Text = "Line 10"},
                new BasicLineViewModel {LineNo = 20, Length = 10, Text = "Line 20"},
                new BasicLineViewModel {LineNo = 30, Length = 10, Text = "Line 30"},
                new BasicLineViewModel {LineNo = 40, Length = 10, Text = "Line 40"},
            };
        }

        /// <summary>
        /// Gets the start address of the BASIC program
        /// </summary>
        public ushort ProgStartAddress
        {
            get
            {
                if (_memory == null) return 0;
                var prog = SystemVariables.Variables.FirstOrDefault(v => v.Name == "PROG")?.Address;
                if (prog == null) return 0;
                return (ushort)(_memory[(ushort)prog] + _memory[(ushort)(prog + 1)] * 0x100);
            }
        }

        /// <summary>
        /// Gets the start address of the BASIC program
        /// </summary>
        public ushort VarsStartAddress
        {
            get
            {
                if (_memory == null) return 0;
                var vars = SystemVariables.Variables.FirstOrDefault(v => v.Name == "VARS")?.Address;
                if (vars == null) return 0;
                return (ushort)(_memory[(ushort)vars] + _memory[(ushort)(vars + 1)] * 0x100);
            }
        }

        /// <summary>
        /// Set the machnine status and notify controls
        /// </summary>
        protected override void OnVmStateChanged(MachineStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);
            if (VmStopped)
            {
                _memory = null;
                return;
            }
            _memory = MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer();
            if (!_tokensRead)
            {
                ReadTokenTable();
                _tokensRead = true;
            }
            DecodeBasicProgram();
        }

        /// <summary>
        /// Reads the table of tokens from the ROM
        /// </summary>
        private void ReadTokenTable()
        {
            _tokens = new List<string>();
            var tokenPtr = MachineViewModel.SpectrumVm.RomInfo.TokenTableAddress;
            tokenPtr++;
            var tokenCount = MachineViewModel.SpectrumVm.RomInfo.TokenCount;
            var token = "";
            while (tokenCount > 0)
            {
                var nextChar = _memory[tokenPtr++];
                if ((nextChar & 0x80) > 0)
                {
                    token += (char) (nextChar & 0xFF7F);
                    _tokens.Add(token);
                    tokenCount--;
                    token = "";
                }
                else
                {
                    token += (char) nextChar;
                }
            }

        }

        public void DecodeBasicProgram()
        {
            ProgramLines.Clear();
            if (VmStopped) return;

            var progStart = ProgStartAddress;
            var varStart = VarsStartAddress;

            if (progStart == 0 || varStart == 0) return;

            while (progStart < varStart)
            {
                progStart = GetBasicLine(progStart, out BasicLineViewModel line);
                ProgramLines.Add(line);
            }
        }

        public ushort GetBasicLine(ushort progStart, out BasicLineViewModel lineVm)
        {
            lineVm = new BasicLineViewModel
            {
                LineNo = _memory[progStart++] * 0x100 + _memory[progStart++],
                Length = _memory[progStart++] + _memory[progStart++],
            };

            var sb = new StringBuilder(256);

            while (_memory[progStart] != 0x0D)
            {
                var nextSymbol = _memory[progStart++];
                if (nextSymbol >= 0x20 && nextSymbol <= 0x7F)
                {
                    // --- Printable character
                    sb.Append((char) nextSymbol);
                }
                else if (nextSymbol >= 0xA5)
                {
                    // --- This is a token
                    sb.Append(_tokens[nextSymbol - 0xA5]);
                    sb.Append(" ");
                }
                else if (nextSymbol == 0x0E)
                {
                    // --- Skip the binary form of a floating point number
                    progStart += 5;
                }
            }

            lineVm.Text = sb.ToString();
            return ++progStart;
        }
    }
}