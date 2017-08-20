using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;

// ReSharper disable AssignNullToNotNullAttribute

namespace Spect.Net.VsPackage.Tools.SpectrumEmulator
{
    /// <summary>
    /// This ROM provider provides a rom from the current project's workspace.
    /// </summary>
    public class ProjectFileRomProvider: IRomProvider
    {
        /// <summary>
        /// Nothing to do when the provider is reset
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <returns>Content of the ROM</returns>
        public RomInfo LoadRom(string romResourceName)
        {
            var rom = VsxPackage.GetPackage<SpectNetPackage>().CurrentWorkspace?.RomItem;
            if (rom == null) return null;

            var romAnnotationFile =
                Path.Combine(
                    Path.GetDirectoryName(rom.Filename),
                    Path.GetFileNameWithoutExtension(rom.Filename)) + ".disann";
            if (!File.Exists(romAnnotationFile)) return null;

            // --- Get the contents of the annotation file
            var serialized = File.ReadAllText(romAnnotationFile);
            var annotations = DisassemblyAnnotation.Deserialize(serialized);
            var result = new RomInfo
            {
                MemorySections = new List<MemorySection>(annotations.MemoryMap),
                LoadBytesInvalidHeaderAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$LoadBytesInvalidHeaderAddress")).Key,
                LoadBytesResumeAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$LoadBytesResumeAddress")).Key,
                LoadBytesRoutineAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$LoadBytesRoutineAddress")).Key,
                SaveBytesRoutineAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$SaveBytesRoutineAddress")).Key,
                SaveBytesResumeAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$SaveBytesResumeAddress")).Key,
                TokenTableAddress = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$TokenTableAddress")).Key,
                TokenCount = annotations.Literals
                    .FirstOrDefault(kvp => kvp.Value.Contains("$TokenCount")).Key,
            };

            // --- Get the content of the ROM file
            using (var stream = new StreamReader(rom.Filename).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                result.RomBytes = bytes;
            }

            return result;
        }
    }
}