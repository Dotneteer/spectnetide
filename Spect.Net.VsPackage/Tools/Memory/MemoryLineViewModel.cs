using System.Collections.Generic;
using System.Text;
using Spect.Net.VsPackage.Utility;
using Spect.Net.Wpf.Mvvm;

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
        public ushort BaseAddress { get; }

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

        public MemoryLineViewModel(ushort addr)
        {
            BaseAddress = addr;
        }

        /// <summary>
        /// Binds this memory line to the specified memory address
        /// </summary>
        /// <param name="memory">Memory array</param>
        public void BindTo(byte[] memory)
        {
            var startAddr = (ushort)(BaseAddress & 0xFFF7);
            var outOf = startAddr + 8 >= 0x10000;
            if (memory == null)
            {
                // --- Manage the dump of disconnected memory
                Addr1 = startAddr.AsHexWord();
                Value0 = Value1 = Value2 = Value3 = Value4 =
                    Value5 = Value6 = Value7 = "--";
                Dump1 = "--------";
                startAddr += 8;
                Addr2 = outOf ? "" : startAddr.AsHexWord();
                Value8 = Value9 = ValueA = ValueB = ValueC =
                    ValueD = ValueE = ValueF = outOf ? "" : "--";
                Dump2 = outOf ? "" : "--------";
                return;
            }

            // --- Memory is connected
            Addr1 = startAddr.AsHexWord();
            Dump1 = DumpValue(memory, startAddr);
            Value0 = memory[startAddr++].AsHexaByte();
            Value1 = memory[startAddr++].AsHexaByte();
            Value2 = memory[startAddr++].AsHexaByte();
            Value3 = memory[startAddr++].AsHexaByte();
            Value4 = memory[startAddr++].AsHexaByte();
            Value5 = memory[startAddr++].AsHexaByte();
            Value6 = memory[startAddr++].AsHexaByte();
            Value7 = memory[startAddr++].AsHexaByte();

            Addr2 = outOf ? "" : startAddr.AsHexWord();
            Dump2 = DumpValue(memory, startAddr);
            Value8 = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            Value9 = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            ValueA = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            ValueB = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            ValueC = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            ValueD = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            ValueE = outOf ? "" : memory[startAddr++].AsHexaByte(); 
            ValueF = outOf ? "" : memory[startAddr].AsHexaByte(); 
        }

        private static string DumpValue(IReadOnlyList<byte> memory, ushort startAddr)
        {
            var sb = new StringBuilder(8);
            for (var i = 0; i < 8; i++)
            {
                var ch = (char)memory[startAddr++];
                sb.Append(char.IsControl(ch) ? '.' : ch);
            }
            return sb.ToString();
        }
    }
}