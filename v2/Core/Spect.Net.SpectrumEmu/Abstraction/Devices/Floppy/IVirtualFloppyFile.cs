using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Floppy
{
    /// <summary>
    /// This interface represents a virtual floppy file.
    /// </summary>
    public interface IVirtualFloppyFile
    {
        /// <summary>
        /// Disk format.
        /// </summary>
        FloppyFormat DiskFormat { get; }

        /// <summary>
        /// The name of the floppy file with full path.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Is the floppy write protected?
        /// </summary>
        bool IsWriteProtected { get; }

        /// <summary>
        /// Signs if the floppy is double-sided.
        /// </summary>
        bool IsDoubleSided { get; }

        /// <summary>
        /// Gets the number of tracks.
        /// </summary>
        byte Tracks { get; }

        /// <summary>
        /// Gets the number of sectors per track.
        /// </summary>
        byte SectorsPerTrack { get; }

        /// <summary>
        /// First sector index of the current format.
        /// </summary>
        byte FirstSectorIndex { get; }

        /// <summary>
        /// Format specifier.
        /// </summary>
        List<byte> FormatSpec { get; }

        /// <summary>
        /// Checks if the current drive is Spectrum compatible.
        /// </summary>
        /// <returns>True, if the format is Spectrum compatible; otherwise, false.</returns>
        bool IsSpectrumVmCompatible();

        /// <summary>
        /// Writes the data to the file.
        /// </summary>
        /// <param name="head">Head parameter (0 or 1).</param>
        /// <param name="track">Track parameter (starts from 0).</param>
        /// <param name="sector">Sector parameter (starts from 1).</param>
        /// <param name="data">Data to write to.</param>
        void WriteData(int head, int track, int sector, byte[] data);

        /// <summary>
        /// Reads data from the file.
        /// </summary>
        /// <param name="head">Head parameter (0 or 1).</param>
        /// <param name="track">Track parameter (starts from 0).</param>
        /// <param name="sector">Sector parameter (starts from 1).</param>
        /// <param name="length">Number of bytes to read.</param>
        byte[] ReadData(int head, int track, int sector, int length);
    }
}