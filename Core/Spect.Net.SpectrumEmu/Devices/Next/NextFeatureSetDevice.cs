using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Next.Palettes;

namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents the feature control registers of Next
    /// </summary>
    public class NextFeatureSetDevice : INextFeatureSetDevice
    {
        // --- The set of registers
        private readonly Dictionary<byte, FeatureControlRegisterBase> _registers = 
            new Dictionary<byte, FeatureControlRegisterBase>();


        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public IDeviceState GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state)
        {
        }

        #region Registers

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

        #endregion

        #region Control set lifecycle methods

        /// <summary>
        /// Initializes a new instance of the Next registers
        /// </summary>
        public NextFeatureSetDevice()
        {
            // --- Create registers and set up their default values
            // --- Setup Register $40
            PaletteIndexRegister = Add<PaletteIndexRegister>();
            PaletteIndexRegister.RegisterValueSet += OnPaletteIndexRegisterSet;

            // --- Setup Register $41
            PaletteValueRegister = Add<PaletteValueRegister>();
            PaletteValueRegister.RegisterValueSet += OnPaletteValueRegisterSet;

            // --- Setup Register $42
            UlaNextInkColorMaskRegister = Add<UlaNextInkColorMaskRegister>();

            // --- Setup Register $43
            UlaNextControlRegister = Add<UlaNextControlRegister>();
            UlaNextControlRegister.RegisterValueSet += OnUlaNextControlRegisterValueSet;

            // --- Setup Register $44
            UlaNextPaletteExtensionRegister = Add<UlaNextPaletteExtensionRegister>();
            UlaNextPaletteExtensionRegister.RegisterValueSet += OnUlaNextPaletteExtensionValueSet;

            // --- Create palettes
            UlaNextFirstPalette = new Palette();
            UlaNextSecondPalette = new Palette();
            Layer2FirstPalette = new Palette();
            Layer2SecondPalette = new Palette();
            SpritesFirstPalette = new Palette();
            SpritesSecondPalette = new Palette();
            InitializePalettes();

            // --- Set ULA Next First palette as the active one
            ActivePalette = UlaNextFirstPalette;
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

        #endregion

        #region Palettes

        /// <summary>
        /// Gets the active palette
        /// </summary>
        public Palette ActivePalette { get; private set; }

        /// <summary>
        /// Firts ULA Next palette
        /// </summary>
        public Palette UlaNextFirstPalette { get; }

        /// <summary>
        /// Second ULA Next palette
        /// </summary>
        public Palette UlaNextSecondPalette { get; }

        /// <summary>
        /// Firts Layer 2 palette
        /// </summary>
        public Palette Layer2FirstPalette { get; }

        /// <summary>
        /// Second Layer 2 palette
        /// </summary>
        public Palette Layer2SecondPalette { get; }

        /// <summary>
        /// Firts Sprites palette
        /// </summary>
        public Palette SpritesFirstPalette { get; }

        /// <summary>
        /// Second Sprites palette
        /// </summary>
        public Palette SpritesSecondPalette { get; }

        /// <summary>
        /// Initializes pallettes to their default values
        /// </summary>
        private void InitializePalettes()
        {
            // --- By default initialize all palette colors
            // --- to colors following the RRRGGGBB0 pattern
            for (var index = 0; index <= 0xFF; index++)
            {
                var i = (byte) index;
                UlaNextFirstPalette[i] =
                    UlaNextSecondPalette[i] =
                        Layer2FirstPalette[i] =
                            Layer2SecondPalette[i] =
                                SpritesFirstPalette[i] =
                                    SpritesSecondPalette[i] = index * 2;
            }

            // --- We fix the color with index 0xFF to be full white
            UlaNextFirstPalette[0xFF] =
                UlaNextSecondPalette[0xFF] =
                    Layer2FirstPalette[0xFF] =
                        Layer2SecondPalette[0xFF] =
                            SpritesFirstPalette[0xFF] =
                                SpritesSecondPalette[0xFF] = 0b111_111_111;

            // --- ULA NEXT: 0x00-0x07 for the standard ink colors (RRRGGGBBB)
            // --- ULA NEXT: 0x80-0x87 for standard paper colors
            UlaNextFirstPalette[0x00] = UlaNextFirstPalette[0x80] = 0b000_000_000;
            UlaNextFirstPalette[0x01] = UlaNextFirstPalette[0x81] = 0b000_000_110;
            UlaNextFirstPalette[0x02] = UlaNextFirstPalette[0x82] = 0b110_000_000;
            UlaNextFirstPalette[0x03] = UlaNextFirstPalette[0x83] = 0b110_000_110;
            UlaNextFirstPalette[0x04] = UlaNextFirstPalette[0x84] = 0b000_110_000;
            UlaNextFirstPalette[0x05] = UlaNextFirstPalette[0x85] = 0b000_110_110;
            UlaNextFirstPalette[0x06] = UlaNextFirstPalette[0x86] = 0b110_110_000;
            UlaNextFirstPalette[0x03] = UlaNextFirstPalette[0x83] = 0b110_110_110;

            // --- ULA NEXT: 0x08-0x0F for the bright ink colors (RRRGGGBBB)
            // --- ULA NEXT: 0x88-0x8F for bright paper colors
            UlaNextFirstPalette[0x08] = UlaNextFirstPalette[0x88] = 0b000_000_000;
            UlaNextFirstPalette[0x09] = UlaNextFirstPalette[0x89] = 0b000_000_111;
            UlaNextFirstPalette[0x0A] = UlaNextFirstPalette[0x8A] = 0b111_000_000;
            UlaNextFirstPalette[0x0B] = UlaNextFirstPalette[0x8B] = 0b111_000_111;
            UlaNextFirstPalette[0x0C] = UlaNextFirstPalette[0x8C] = 0b000_111_000;
            UlaNextFirstPalette[0x0D] = UlaNextFirstPalette[0x8D] = 0b000_111_111;
            UlaNextFirstPalette[0x0E] = UlaNextFirstPalette[0x8E] = 0b111_111_000;
            UlaNextFirstPalette[0x0F] = UlaNextFirstPalette[0x83] = 0b111_111_111;
        }

        /// <summary>
        /// Signs the Register $44 will receive the first byte
        /// </summary>
        private void OnPaletteIndexRegisterSet(object sender, RegisterSetEventArgs e)
        {
            UlaNextPaletteExtensionRegister.FirstByteSet = false;
        }

        /// <summary>
        /// Sets the active palette whenever the value of ULA Next 
        /// control register ($43) is written
        /// </summary>
        private void OnUlaNextControlRegisterValueSet(object sender, RegisterSetEventArgs e)
        {
            switch (e.Value >> 4)
            {
                case 1:
                    ActivePalette = Layer2FirstPalette;
                    break;
                case 2:
                    ActivePalette = SpritesFirstPalette;
                    break;
                case 4:
                    ActivePalette = UlaNextSecondPalette;
                    break;
                case 5:
                    ActivePalette = Layer2SecondPalette;
                    break;
                case 6:
                    ActivePalette = SpritesSecondPalette;
                    break;
                default:
                    ActivePalette = UlaNextFirstPalette;
                    break;
            }
        }

        /// <summary>
        /// Sets the palette value (8 bit) when Register $41 is set
        /// </summary>
        private void OnPaletteValueRegisterSet(object sender, RegisterSetEventArgs e)
        {
            // --- Set the first 8 bits of palette color: RRRGGGBBX,
            // --- where X is the bitwise or of the two B bits
            var newValue = e.Value << 1 
                | ((e.Value & 0x03) == 0 ? 0x00 : 0x01);
            ActivePalette[PaletteIndexRegister.LastValue] = newValue;

            // --- Palette index is automatically incremented
            PaletteIndexRegister.Set((byte)(PaletteIndexRegister.LastValue + 1));
        }

        /// <summary>
        /// Sets the 8-bit + 1-bit extended palette value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUlaNextPaletteExtensionValueSet(object sender, RegisterSetEventArgs e)
        {
            if (UlaNextPaletteExtensionRegister.FirstByteSet)
            {
                // --- We receive the 9th bit of the palette value
                var index = PaletteIndexRegister.LastValue;
                ActivePalette[index] = (ActivePalette[index] &0x1FE) | (e.Value & 0x01);

                // --- Palette index is automatically incremented
                PaletteIndexRegister.Set((byte)(PaletteIndexRegister.LastValue + 1));
                UlaNextPaletteExtensionRegister.FirstByteSet = false;
            }
            else
            {
                // --- Set the first 8 bits of palette color: RRRGGGBBX,
                // --- where X is the bitwise or of the two B bits
                var newValue = e.Value << 1
                               | ((e.Value & 0x03) == 0 ? 0x00 : 0x01);
                ActivePalette[PaletteIndexRegister.LastValue] = newValue;
                UlaNextPaletteExtensionRegister.FirstByteSet = true;
            }
        }

        #endregion

        #region Helpers

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

        #endregion

    }
}