using System.Collections.Generic;
using System.Linq;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Devices.Rom
{
    /// <summary>
    /// This class provides 
    /// </summary>
    public class SpectrumRomDevice: IRomDevice
    {
        /// <summary>
        /// The address to terminate the data block load when the header is
        /// invalid
        /// </summary>
        public const string LOAD_BYTES_INVALID_HEADER_ADDRESS = "$LoadBytesInvalidHeaderAddress";

        /// <summary>
        /// The address of the main execution cycle.
        /// invalid
        /// </summary>
        public const string MAIN_EXEC_ADDRESS = "$MainExecAddress";

        /// <summary>
        /// The address to resume after a hooked LOAD_BYTES operation
        /// </summary>
        public const string LOAD_BYTES_RESUME_ADDRESS = "$LoadBytesResumeAddress";
        
        /// <summary>
        /// The LOAD_BYTES routine address in the ROM
        /// </summary>
        public const string LOAD_BYTES_ROUTINE_ADDRESS = "$LoadBytesRoutineAddress";

        /// <summary>
        /// The SAVE_BYTES routine address in the ROM
        /// </summary>
        public const string SAVE_BYTES_ROUTINE_ADDRESS = "$SaveBytesRoutineAddress";
        
        /// <summary>
        /// The start address of the token table
        /// </summary>
        protected const string TOKEN_TABLE_ADDRESS = "$TokenTableAddress";

        /// <summary>
        /// The number of token in the token table
        /// </summary>
        protected const string TOKEN_COUNT = "$TokenCount";

        /// <summary>
        /// The token table property key
        /// </summary>
        public const string TOKEN_TABLE_KEY = "TokenTable";

        /// <summary>
        /// Get the offset of tokens in BASIC listings
        /// </summary>
        protected const string TOKEN_OFFSET = "$TokenOffset";

        private IRomProvider _romProvider;
        private IRomConfiguration _romConfiguration;
        private readonly Dictionary<(string name, int romIndex), ushort> _knownAddresses
            = new Dictionary<(string name, int romIndex), ushort>();
        private readonly Dictionary<(string name, int romIndex), object> _properties
            = new Dictionary<(string name, int romIndex), object>();
        private byte[][] _romBytes;
        private DisassemblyAnnotation[] _romAnnotations;

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
            var romInfo = hostVm.GetDeviceInfo<IRomDevice>();
            _romProvider = (IRomProvider)romInfo.Provider;
            _romConfiguration = (IRomConfiguration) romInfo.ConfigurationData;

            // --- Init the ROM contents and ROM annotations
            var roms = _romConfiguration.NumberOfRoms;
            if (roms > 16) roms = 16;
            _romBytes = new byte[roms][];
            _romAnnotations = new DisassemblyAnnotation[roms];
            if (roms == 1)
            {
                _romBytes[0] = _romProvider.LoadRomBytes(_romConfiguration.RomName);
                _romAnnotations[0] = DisassemblyAnnotation.Deserialize(
                    _romProvider.LoadRomAnnotations(_romConfiguration.RomName));
            }
            else
            {
                for (var i = 0; i < roms; i++)
                {
                    _romBytes[i] = _romProvider.LoadRomBytes(_romConfiguration.RomName, i);
                    _romAnnotations[i] = DisassemblyAnnotation.Deserialize(
                        _romProvider.LoadRomAnnotations(_romConfiguration.RomName, i));
                }
            }

            ProcessRoms(_romBytes, _romAnnotations);
        }

        /// <summary>
        /// Override this method to define how to process the ROM contents
        /// </summary>
        /// <param name="romBytes">ROM contents</param>
        /// <param name="romAnnotations">ROM annotations</param>
        protected virtual void ProcessRoms(byte[][] romBytes, DisassemblyAnnotation[] romAnnotations)
        {
            ProcessSpectrum48Props(romBytes, romAnnotations);
        }

        /// <summary>
        /// Process the specified ROM page as a Spectrum48 ROM page
        /// </summary>
        /// <param name="romBytes">Contents of the ROMs</param>
        /// <param name="romAnnotations">ROM annotations</param>
        protected void ProcessSpectrum48Props(byte[][] romBytes, 
            DisassemblyAnnotation[] romAnnotations)
        {
            var romPage = _romConfiguration.Spectrum48RomIndex;
            var annotations = romAnnotations[romPage];

            // --- Get SAVE/LOAD vectors
            _knownAddresses.Add((LOAD_BYTES_INVALID_HEADER_ADDRESS, romPage), annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(LOAD_BYTES_INVALID_HEADER_ADDRESS)).Key);
            _knownAddresses.Add((MAIN_EXEC_ADDRESS, romPage), annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(MAIN_EXEC_ADDRESS)).Key);
            _knownAddresses.Add((LOAD_BYTES_RESUME_ADDRESS, romPage), annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(LOAD_BYTES_RESUME_ADDRESS)).Key);
            _knownAddresses.Add((LOAD_BYTES_ROUTINE_ADDRESS, romPage), annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(LOAD_BYTES_ROUTINE_ADDRESS)).Key);
            _knownAddresses.Add((SAVE_BYTES_ROUTINE_ADDRESS, romPage), annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(SAVE_BYTES_ROUTINE_ADDRESS)).Key);

            // --- Get token vectors
            var tokenTableAddress = annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(TOKEN_TABLE_ADDRESS)).Key;
            _knownAddresses.Add((TOKEN_TABLE_ADDRESS, romPage), tokenTableAddress);
            var tokenCount = annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(TOKEN_COUNT)).Key;
            _knownAddresses.Add((TOKEN_COUNT, romPage), tokenCount);
            _knownAddresses.Add((TOKEN_OFFSET, romPage), annotations.Literals
                .FirstOrDefault(kvp => kvp.Value.Contains(TOKEN_OFFSET)).Key);

            var rom = romBytes[romPage];
            var tokenTable = new List<string>();

            var tokenPtr = tokenTableAddress;
            tokenPtr++;
            var token = "";
            while (tokenCount > 0)
            {
                var nextChar = rom[tokenPtr++];
                if ((nextChar & 0x80) > 0)
                {
                    token += (char)(nextChar & 0xFF7F);
                    tokenTable.Add(token);
                    tokenCount--;
                    token = "";
                }
                else
                {
                    token += (char)nextChar;
                }
            }
            _properties.Add((TOKEN_TABLE_KEY, romPage), tokenTable);
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Gets the binary contents of the rom
        /// </summary>
        /// <param name="romIndex">Index of the ROM, by default, 0</param>
        /// <returns>Byte array that represents the ROM bytes</returns>
        public byte[] GetRomBytes(int romIndex = 0) => _romBytes[romIndex];

        /// <summary>
        /// Gets a known address of a particular ROM
        /// </summary>
        /// <param name="key">Known address key</param>
        /// <param name="romIndex">Index of the ROM, by default, 0</param>
        /// <returns>Address, if found; otherwise, null</returns>
        public ushort? GetKnownAddress(string key, int romIndex = 0) => 
            _knownAddresses.TryGetValue((key, romIndex), out var value) 
                ? (ushort?) value : null;

        /// <summary>
        /// Gets a property of the ROM (depends on virtual machine model)
        /// </summary>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value if found</param>
        /// <param name="romIndex">Index of the ROM, by default, 0</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetProperty<TProp>(string key, out TProp value, int romIndex = 0)
        {
            var result = _properties.TryGetValue((key, romIndex), out var storedValue);
            value = result ? (TProp) storedValue : default(TProp);
            return result;
        }
    }
}