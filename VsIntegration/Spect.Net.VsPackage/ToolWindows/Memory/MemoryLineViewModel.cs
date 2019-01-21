using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.VsPackage.Utility;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This view model represents a memory line with 2x8 bytes displayed
    /// </summary>
    public class MemoryLineViewModel: EnhancedViewModelBase
    {
        private static readonly Regex s_ColorSpecRegex = new Regex(@"^\s*([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})$");

        private static Brush s_BcBrush;
        private static Brush s_DeBrush;
        private static Brush s_HlBrush;
        private static Brush s_IxBrush;
        private static Brush s_IyBrush;
        private static Brush s_SpBrush;
        private static Brush s_PcBrush;

        private readonly Registers _regs;
        private string _addr1;
        private string _value0;
        private string _value1;
        private string _value2;
        private string _value3;
        private string _value4;
        private string _value5;
        private string _value6;
        private string _value7;
        private string _dump1;
        private string _addr2;
        private string _value8;
        private string _value9;
        private string _valueA;
        private string _valueB;
        private string _valueC;
        private string _valueD;
        private string _valueE;
        private string _valueF;
        private string _dump2;
        private Brush _mark0;
        private Brush _mark1;
        private Brush _mark2;
        private Brush _mark3;
        private Brush _mark4;
        private Brush _mark5;
        private Brush _mark6;
        private Brush _mark7;
        private Brush _mark8;
        private Brush _mark9;
        private Brush _markA;
        private Brush _markB;
        private Brush _markC;
        private Brush _markD;
        private Brush _markE;
        private Brush _markF;

        /// <summary>
        /// Base address of the memory line
        /// </summary>
        public int BaseAddress { get; }

        /// <summary>
        /// Top address of the memory line
        /// </summary>
        public int TopAddress { get; }

        public string Addr1
        {
            get => _addr1;
            set => Set(ref _addr1, value);
        }

        public string Value0
        {
            get => _value0;
            set => Set(ref _value0, value);
        }

        public string Value1
        {
            get => _value1;
            set => Set(ref _value1, value);
        }

        public string Value2
        {
            get => _value2;
            set => Set(ref _value2, value);
        }

        public string Value3
        {
            get => _value3;
            set => Set(ref _value3, value);
        }

        public string Value4
        {
            get => _value4;
            set => Set(ref _value4, value);
        }

        public string Value5
        {
            get => _value5;
            set => Set(ref _value5, value);
        }

        public string Value6
        {
            get => _value6;
            set => Set(ref _value6, value);
        }

        public string Value7
        {
            get => _value7;
            set => Set(ref _value7, value);
        }

        public string Dump1
        {
            get => _dump1;
            set => Set(ref _dump1, value);
        }

        public string Addr2
        {
            get => _addr2;
            set => Set(ref _addr2, value);
        }

        public string Value8
        {
            get => _value8;
            set => Set(ref _value8, value);
        }

        public string Value9
        {
            get => _value9;
            set => Set(ref _value9, value);
        }

        public string ValueA
        {
            get => _valueA;
            set => Set(ref _valueA, value);
        }

        public string ValueB
        {
            get => _valueB;
            set => Set(ref _valueB, value);
        }

        public string ValueC
        {
            get => _valueC;
            set => Set(ref _valueC, value);
        }

        public string ValueD
        {
            get => _valueD;
            set => Set(ref _valueD, value);
        }

        public string ValueE
        {
            get => _valueE;
            set => Set(ref _valueE, value);
        }

        public string ValueF
        {
            get => _valueF;
            set => Set(ref _valueF, value);
        }

        public string Dump2
        {
            get => _dump2;
            set => Set(ref _dump2, value);
        }

        public Brush Mark0
        {
            get => _mark0;
            set => Set( ref _mark0, value);
        }

        public Brush Mark1
        {
            get => _mark1;
            set => Set(ref _mark1, value);
        }

        public Brush Mark2
        {
            get => _mark2;
            set => Set(ref _mark2, value);
        }

        public Brush Mark3
        {
            get => _mark3;
            set => Set(ref _mark3, value);
        }

        public Brush Mark4
        {
            get => _mark4;
            set => Set(ref _mark4, value);
        }

        public Brush Mark5
        {
            get => _mark5;
            set => Set(ref _mark5, value);
        }

        public Brush Mark6
        {
            get => _mark6;
            set => Set(ref _mark6, value);
        }

        public Brush Mark7
        {
            get => _mark7;
            set => Set(ref _mark7, value);
        }

        public Brush Mark8
        {
            get => _mark8;
            set => Set(ref _mark8, value);
        }

        public Brush Mark9
        {
            get => _mark9;
            set => Set(ref _mark9, value);
        }

        public Brush MarkA
        {
            get => _markA;
            set => Set(ref _markA, value);
        }

        public Brush MarkB
        {
            get => _markB;
            set => Set(ref _markB, value);
        }

        public Brush MarkC
        {
            get => _markC;
            set => Set(ref _markC, value);
        }

        public Brush MarkD
        {
            get => _markD;
            set => Set(ref _markD, value);
        }

        public Brush MarkE
        {
            get => _markE;
            set => Set(ref _markE, value);
        }

        public Brush MarkF
        {
            get => _markF;
            set => Set(ref _markF, value);
        }


        public MemoryLineViewModel()
        {
            if (IsInDesignMode)
            {
                Addr1 = "4000";
                Value0 = "01";
                Value1 = "12";
                Value2 = "23";
                Value3 = "34";
                Value4 = "45";
                Value5 = "56";
                Value6 = "67";
                Value7 = "78";
                Dump1 = "..34..78";
                Addr2 = "4008";
                Value8 = "89";
                Value9 = "9A";
                ValueA = "AB";
                ValueB = "BC";
                ValueC = "CD";
                ValueD = "DE";
                ValueE = "EF";
                ValueF = "F0";
                Dump2 = "..AB..EF";
            }

            Mark0 = Mark1 = Mark2 = Mark3 = Mark4 = Mark5 = Mark6 = Mark7 =
            Mark8 = Mark9 = MarkA = MarkB = MarkC = MarkD = MarkE = MarkF =
                Brushes.Transparent;
        }

        /// <summary>
        /// Creates a memory line with the specified base address and top address
        /// </summary>
        /// <param name="regs">Z80 register current values</param>
        /// <param name="baseAddr">Memory base address</param>
        /// <param name="topAddress">Memory top address</param>
        public MemoryLineViewModel(Registers regs, int baseAddr, int topAddress = 0xFFFF)
        {
            _regs = regs;
            BaseAddress = baseAddr;
            TopAddress = topAddress;
        }

        /// <summary>
        /// Binds this memory line to the specified memory address
        /// </summary>
        /// <param name="memory">Memory array</param>
        public void BindTo(byte[] memory)
        {
            Addr1 = BaseAddress.AsHexWord();
            Dump1 = DumpValue(memory, BaseAddress);
            Value0 = GetByte(memory, 0);
            if (_regs != null)
            {
                Mark0 = s_BcBrush;
            }
            Value1 = GetByte(memory, 1);
            Value2 = GetByte(memory, 2);
            Value3 = GetByte(memory, 3);
            Value4 = GetByte(memory, 4);
            Value5 = GetByte(memory, 5);
            Value6 = GetByte(memory, 6);
            Value7 = GetByte(memory, 7);

            if (BaseAddress + 8 > TopAddress) return;

            Addr2 = (BaseAddress + 8).AsHexWord();
            Dump2 = DumpValue(memory, BaseAddress + 8);
            Value8 = GetByte(memory, 8);
            Value9 = GetByte(memory, 9);
            ValueA = GetByte(memory, 10);
            ValueB = GetByte(memory, 11);
            ValueC = GetByte(memory, 12);
            ValueD = GetByte(memory, 13);
            ValueE = GetByte(memory, 14);
            ValueF = GetByte(memory, 15);
        }

        /// <summary>
        /// Refreshes the register highlight brush colors from SpectNet options
        /// </summary>
        public static void RefreshRegisterBrushes()
        {
            s_BcBrush = GetBrushFromColor(SpectNetPackage.Default.Options.BcColor);
            s_DeBrush = GetBrushFromColor(SpectNetPackage.Default.Options.DeColor);
            s_HlBrush = GetBrushFromColor(SpectNetPackage.Default.Options.HlColor);
            s_IxBrush = GetBrushFromColor(SpectNetPackage.Default.Options.IxColor);
            s_IyBrush = GetBrushFromColor(SpectNetPackage.Default.Options.IyColor);
            s_SpBrush = GetBrushFromColor(SpectNetPackage.Default.Options.SpColor);
            s_PcBrush = GetBrushFromColor(SpectNetPackage.Default.Options.PcColor);
        }

        private static Brush GetBrushFromColor(string color)
        {
            var prop = typeof(Brushes).GetProperty(color,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            if (prop != null && prop.PropertyType == typeof(SolidColorBrush))
            {
                return (SolidColorBrush) prop.GetValue(null);
            }

            var match = s_ColorSpecRegex.Match(color);
            if (!match.Success)
            {
                return Brushes.Green;
            }
            return Brushes.Transparent;
        }

        private string DumpValue(IReadOnlyList<byte> memory, int startAddr)
        {
            var sb = new StringBuilder(8);
            for (var i = 0; i < 8; i++)
            {
                if (startAddr + i > TopAddress) break;
                if (memory == null)
                {
                    sb.Append('-');
                }
                else
                {
                    var ch = (char) memory[startAddr + i];
                    sb.Append(char.IsControl(ch) ? '.' : ch);
                }
            }
            return sb.ToString();
        }

        private string GetByte(byte[] memory, int offset)
        {
            var memAddr = BaseAddress + offset;
            return memAddr <= TopAddress ? (memory == null ? "--" : memory[memAddr].AsHexaByte()) : null;
        }
    }
}