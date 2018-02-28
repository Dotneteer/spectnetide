using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents the feature control registers of Next
    /// </summary>
    public class NextFeatureControlSet
    {
        // --- The set of registers
        private readonly Dictionary<byte, FeatureControlRegisterBase> _registers = 
            new Dictionary<byte, FeatureControlRegisterBase>();

        /// <summary>
        /// Gets the last set register index
        /// </summary>
        public byte LastRegisterIndexSet { get; private set; }

        /// <summary>
        /// Palette Index Register ($40)
        /// </summary>
        public PaletteIndexRegister PaletteIndexRegister { get; }

        /// <summary>
        /// Palette Value Register ($41)
        /// </summary>
        public PaletteValueRegister PaletteValueRegister { get; }

        /// <summary>
        /// ULANext Ink Color Mask ($42)
        /// </summary>
        public UlaNextInkColorMaskRegister UlaNextInkColorMaskRegister { get; }

        /// <summary>
        /// ULANext Control Register ($43)
        /// </summary>
        public UlaNextControlRegister UlaNextControlRegister { get; }

        /// <summary>
        /// ULANext Palette Extension ($44);
        /// </summary>
        public UlaNextPaletteExtensionRegister UlaNextPaletteExtensionRegister { get; }

        /// <summary>
        /// Initializes a new instance of the Next registers
        /// </summary>
        public NextFeatureControlSet()
        {
            PaletteIndexRegister = Add<PaletteIndexRegister>();
            PaletteValueRegister = Add<PaletteValueRegister>();
            UlaNextInkColorMaskRegister = Add<UlaNextInkColorMaskRegister>();
            UlaNextControlRegister = Add<UlaNextControlRegister>();
            UlaNextPaletteExtensionRegister = Add<UlaNextPaletteExtensionRegister>();
        }

        /// <summary>
        /// Sets the register index for the next SetRegisterValue operation
        /// </summary>
        /// <param name="index"></param>
        public void SetRegisterIndex(byte index)
        {
            LastRegisterIndexSet = index;
        }

        /// <summary>
        /// Sets the value of the register specified by the latest
        /// SetRegisterIndex call
        /// </summary>
        /// <param name="value">Register value to set</param>
        public void SetRegisterValue(byte value)
        {
            if (_registers.TryGetValue(LastRegisterIndexSet, out var register))
            {
                register.Write(value);
            }
        }

        /// <summary>
        /// Gets the value of the register specified by the latest
        /// SetRegisterIndex call
        /// </summary>
        /// <remarks>If the specified register is not supported, returns 0xFF</remarks>
        public byte GetRegisterValue()
        {
            return _registers.TryGetValue(LastRegisterIndexSet, out var register)
                ? register.Read()
                : (byte)0xFF;
        }

        /// <summary>
        /// Adds a new register to the set
        /// </summary>
        /// <typeparam name="TReg">Register type</typeparam>
        /// <returns>Newly created register instance</returns>
        private TReg Add<TReg>()
            where TReg: FeatureControlRegisterBase, new()
        {
            var register = new TReg();
            _registers.Add(register.Id, register);
            return register;
        }
    }
}