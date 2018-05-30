using System;
using System.IO;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.VfddEditor
{
    /// <summary>
    /// Editor pane class for virtual floppy disk file viewer
    /// </summary>
    public class VfddEditorPane : EditorPaneBase<VfddEditorFactory, VfddEditorControl>
    {
        private string _loadPath;
        private VfddEditorViewModel _vm;

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public override string FileExtensionUsed => ".vfdd";

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName"></param>
        protected override void LoadFile(string fileName)
        {
            _loadPath = fileName;
            _vm = new VfddEditorViewModel();
            try
            {
                var floppyFile = VirtualFloppyFile.OpenFloppyFile(fileName);
                _vm.IsValidFormat = true;
                switch (floppyFile.DiskFormat)
                {
                    case FloppyFormat.SpectrumP3:
                        _vm.DiskFormat = "Spectrum +3";
                        break;
                    case FloppyFormat.CpcData:
                        _vm.DiskFormat = "Amstrad CPC data only";
                        break;
                    case FloppyFormat.CpcSystem:
                        _vm.DiskFormat = "Amstrad CPC system";
                        break;
                    case FloppyFormat.Pcw:
                        _vm.DiskFormat = "Standard PCW range";
                        break;
                    default:
                        _vm.DiskFormat = "Unknown";
                        break;
                }
                switch (floppyFile.FormatSpec[1] & 0xFF)
                {
                    case 0:
                        _vm.Sideness = "Single sided";
                        break;
                    case 1:
                        _vm.Sideness = "Double sided (alternating sides)";
                        break;
                    default:
                        _vm.Sideness = "Double sided (successive sides)";
                        break;
                }
                _vm.TracksPerSide = floppyFile.Tracks;
                _vm.SectorsPerTrack = floppyFile.SectorsPerTrack;
                _vm.SectorSize = 1 << (floppyFile.FormatSpec[4] + 7);
                _vm.ReservedTracks = floppyFile.FormatSpec[5];
                _vm.BlockSize = 1 << (floppyFile.FormatSpec[6] + 7);
                _vm.DirectoryBlocks = floppyFile.FormatSpec[7];
                _vm.ReadWriteGapLength = floppyFile.FormatSpec[8];
                _vm.FormatGapLength = floppyFile.FormatSpec[9];
                var capacity = floppyFile.Tracks
                               * (floppyFile.IsDoubleSided ? 2 : 1)
                               * floppyFile.SectorsPerTrack
                               * _vm.SectorSize
                               / 1024;
                _vm.Capacity = $"{capacity} KBytes";
            }
            catch (Exception)
            {
                _vm.IsValidFormat = false;
            }
        }

        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = _vm;
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName"></param>
        public override void SaveFile(string fileName)
        {
            File.Copy(_loadPath, fileName);
        }
    }
}