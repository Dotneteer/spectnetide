using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Spect.Net.SpectrumEmu.Devices.Rom;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// This view model represent the ZX Spectrum BASIC List
    /// </summary>
    public class BasicListViewModel: SpectrumGenericToolWindowViewModel
    {
        private readonly List<string> _tokens;

        /// <summary>
        /// The memory address to decode the basic listing from
        /// </summary>
        public byte[] Memory { get; }

        /// <summary>
        /// The offset where the Basic program starts in the memory
        /// </summary>
        public ushort StartOffset { get; }

        /// <summary>
        /// The offset wher the basic program ends in the memory
        /// </summary>
        public ushort EndOffset { get; }

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
        /// Instantiates this view model with the specified attributes
        /// </summary>
        /// <param name="memory">Memory array</param>
        /// <param name="startOffset">Start offset of the BASIC code</param>
        /// <param name="endOffset">End offset of the BASIC Code (exclusive)</param>
        public BasicListViewModel(byte[] memory, ushort startOffset, ushort endOffset): this()
        {
            var romDevice = MachineViewModel?.SpectrumVm?.RomDevice;
            if (romDevice != null)
            {
                romDevice.GetProperty<List<string>>(SpectrumRomDevice.TOKEN_TABLE_KEY, out var tokenList,
                    romDevice.HostVm.RomConfiguration.Spectrum48RomIndex);
                {
                    _tokens = tokenList;
                }
            }
            Memory = memory;
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        /// <summary>
        /// Decodes the BASIC program in the memory
        /// </summary>
        public void DecodeBasicProgram()
        {
            var progStart = StartOffset;
            var progEnd = EndOffset;

            if (progStart == 0 || progEnd == 0) return;

            while (progStart < progEnd)
            {
                progStart = GetBasicLine(progStart, out BasicLineViewModel line);
                ProgramLines.Add(line);
            }
        }

        /// <summary>
        /// Gets the specified BASIC code line
        /// </summary>
        /// <param name="progStart">Code line address in the memory</param>
        /// <param name="lineVm">BASIC line view model</param>
        /// <returns></returns>
        public ushort GetBasicLine(ushort progStart, out BasicLineViewModel lineVm)
        {
            lineVm = new BasicLineViewModel
            {
                LineNo = Memory[progStart++] * 0x100 + Memory[progStart++],
                Length = Memory[progStart++] + Memory[progStart++] * 0x100
            };
            var lineEnd = progStart + lineVm.Length;
            if (lineEnd > Memory.Length - 1)
            {
                lineEnd = Memory.Length - 1;
            }

            var spaceBeforeToken = false;
            var sb = new StringBuilder(256);

            while (progStart < lineEnd)
            {
                var nextSymbol = Memory[progStart++];
                if (nextSymbol >= 0xA5)
                {
                    // --- This is a token
                    if (spaceBeforeToken)
                    {
                        sb.Append(" ");
                    }
                    var tokenCode = nextSymbol - 0xA5;
                    var token = _tokens[tokenCode];
                    sb.Append(token);
                    if (tokenCode > 2 && char.IsLetter(token[token.Length - 1]))
                    {
                        sb.Append(" ");
                        spaceBeforeToken = false;
                    }
                    continue;
                }

                // --- Whatever we print, the next token needs a space
                spaceBeforeToken = true;

                if (nextSymbol >= 0x20 && nextSymbol <= 0x7F)
                {
                    // --- Printable character
                    sb.Append((char)nextSymbol);
                    continue;
                }

                if (nextSymbol == 0x0D)
                {
                    continue;
                }

                if (nextSymbol == 0x0E)
                {
                    // --- Skip the binary form of a floating point number
                    progStart += 5;
                    continue;
                }

                // --- Non-printable character, let's display it with an escape sequence
                sb.Append($"°{nextSymbol:X2}°");
            }

            lineVm.Text = sb.ToString();
            return progStart;
        }
    }
}