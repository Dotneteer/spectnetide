using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.VsPackage.VsxLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// This view model represent the ZX Spectrum BASIC List
    /// </summary>
    public class BasicListViewModel : SpectrumToolWindowViewModelBase
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
        /// Signs if ZX BASIC should be mimicked
        /// </summary>
        public bool MimicZxBasic { get; set; }

        /// <summary>
        /// Program lines decoded
        /// </summary>
        public ObservableCollection<BasicLineViewModel> ProgramLines { get; set; } = new ObservableCollection<BasicLineViewModel>();

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
        public BasicListViewModel(byte[] memory, ushort startOffset, ushort endOffset, bool mimicZxBasic = false) : this()
        {
            var romDevice = SpectrumVm?.RomDevice;
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
            MimicZxBasic = mimicZxBasic;
        }

        /// <summary>
        /// Decodes the BASIC program in the memory
        /// </summary>
        public void DecodeBasicProgram(List<BasicLineViewModel> list = null)
        {
            var progStart = StartOffset;
            var progEnd = EndOffset;

            if (progStart == 0 || progEnd == 0) return;

            while (progStart < progEnd)
            {
                progStart = GetBasicLine(progStart, out BasicLineViewModel line);
                if (list != null)
                {
                    list.Add(line);
                }
                else
                {
                    ProgramLines.Add(line);
                }
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
            var withinQuotes = false;
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

                // --- Skip ENTER
                if (nextSymbol == 0x0D)
                {
                    continue;
                }

                // --- Skip binary number characters
                if (nextSymbol == 0x0E)
                {
                    // --- Skip the binary form of a floating point number
                    progStart += 5;
                    continue;
                }

                if (nextSymbol == 0x22)
                {
                    // --- Printable character
                    sb.Append((char)nextSymbol);
                    withinQuotes = !withinQuotes;
                    continue;
                }

                if (MimicZxBasic && !withinQuotes && nextSymbol < 0x20)
                {
                    // --- Skip all remaining control characters in ZX BASIC mode
                    continue;
                }

                if (nextSymbol >= 0x20 && nextSymbol <= 0x7F)
                {
                    // --- Printable character
                    if (withinQuotes)
                    {
                        if (nextSymbol == (int)'\\')
                        {
                            sb.Append(MimicZxBasic ? "\\\\" : "\\");
                            continue;
                        }
                        else if (nextSymbol == 0x60)
                        {
                            sb.Append(MimicZxBasic ? "\\`" : "Ł");
                            continue;
                        }
                        else if (nextSymbol == 0x7f)
                        {
                            sb.Append(MimicZxBasic ? "\\*" : "©");
                            continue;
                        }
                    } 
                    else
                    {
                        if (nextSymbol == (int)'\\')
                        {
                            sb.Append("\\");
                            continue;
                        }
                        else if (nextSymbol == 0x60)
                        {
                            sb.Append("Ł");
                            continue;
                        }
                        else if (nextSymbol == 0x7f)
                        {
                            sb.Append("©");
                            continue;
                        }
                    }
                    sb.Append((char)nextSymbol);
                    continue;
                }

                // --- Non-printable character, let's display it with an escape sequence
                if (MimicZxBasic && withinQuotes)
                {
                    if (nextSymbol >= 0x90 && nextSymbol <= 0xA4)
                    {
                        // --- Handle UDG chars in ZX BASIC
                        sb.Append($"\\{(char)(nextSymbol - 0x90 + 'A')}");
                        continue;
                    }

                    var lookahead = progStart < Memory.Length ? Memory[progStart] : -1;
                    switch (nextSymbol)
                    {
                        case 0x10:
                            if (lookahead >= 0)
                            {
                                sb.Append("\\{i" + lookahead + "}");
                                progStart++;
                            }
                            break;
                        case 0x11:
                            if (lookahead >= 0)
                            {
                                sb.Append("\\{p" + lookahead + "}");
                                progStart++;
                            }
                            break;
                        case 0x12:
                            if (lookahead >= 0)
                            {
                                sb.Append("\\{f" + lookahead + "}");
                                progStart++;
                            }
                            break;
                        case 0x13:
                            if (lookahead >= 0)
                            {
                                sb.Append("\\{b" + lookahead + "}");
                                progStart++;
                            }
                            break;
                        case 0x80:
                            sb.Append("\\  ");
                            break;
                        case 0x81:
                            sb.Append("\\ '");
                            break;
                        case 0x82:
                            sb.Append("\\' ");
                            break;
                        case 0x83:
                            sb.Append("\\''");
                            break;
                        case 0x84:
                            sb.Append("\\ .");
                            break;
                        case 0x85:
                            sb.Append("\\ :");
                            break;
                        case 0x86:
                            sb.Append("\\'.");
                            break;
                        case 0x87:
                            sb.Append("\\':");
                            break;
                        case 0x88:
                            sb.Append("\\. ");
                            break;
                        case 0x89:
                            sb.Append("\\.'");
                            break;
                        case 0x8a:
                            sb.Append("\\: ");
                            break;
                        case 0x8b:
                            sb.Append("\\:'");
                            break;
                        case 0x8c:
                            sb.Append("\\..");
                            break;
                        case 0x8d:
                            sb.Append("\\.:");
                            break;
                        case 0x8e:
                            sb.Append("\\:.");
                            break;
                        case 0x8f:
                            sb.Append("\\::");
                            break;
                        default:
                            sb.Append($"#{nextSymbol & 0xff}");
                            break;
                    }
                }
                else
                {
                    sb.Append($"°{nextSymbol:X2}°");
                }
            }

            lineVm.Text = sb.ToString();
            return progStart;
        }

        /// <summary>
        /// Exports the list to the specified file
        /// </summary>
        /// <param name="filename"></param>
        public void ExportToFile(string filename)
        {
            var listing = new StringBuilder(4096);
            foreach (var line in ProgramLines)
            {
                listing.Append(line.LineNo);
                listing.Append(" ");
                listing.AppendLine(line.Text);
            }
            try
            {
                var dirName = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.WriteAllText(filename, listing.ToString());
            }
            catch (Exception ex)
            {
                VsxDialogs.Show($"Error while exporting to file {filename}: {ex.Message}",
                    "Export BASIC listing error.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Compares this view model with another
        /// </summary>
        /// <param name="other">Other view model</param>
        /// <returns>True, if the two view models are equal; otherwise, false</returns>
        public static bool Compare(BasicListViewModel thisModel, BasicListViewModel other)
        {
            if (other == null) return false;
            if (other.ProgramLines.Count != thisModel.ProgramLines.Count) return false;
            for (var i = 0; i < thisModel.ProgramLines.Count; i++)
            {
                var otherItem = other.ProgramLines[i];
                var thisItem = thisModel.ProgramLines[i];
                if (otherItem.LineNo != thisItem.LineNo ||
                    otherItem.Length != thisItem.Length ||
                    otherItem.Text != thisItem.Text)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
