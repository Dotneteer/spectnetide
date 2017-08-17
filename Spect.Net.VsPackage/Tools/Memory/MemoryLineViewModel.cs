using System.Collections.Generic;
using System.Text;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.Tools.Memory
{
    /// <summary>
    /// This view model represents a memory line with 2x8 bytes displayed
    /// </summary>
    public class MemoryLineViewModel: EnhancedViewModelBase
    {
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
        }

        /// <summary>
        /// Creates a memory line with the specified base address and top address
        /// </summary>
        /// <param name="baseAddr">Memory base address</param>
        /// <param name="topAddress">Memory top address</param>
        public MemoryLineViewModel(int baseAddr, int topAddress = 0xFFFF)
        {
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