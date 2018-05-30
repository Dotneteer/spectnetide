using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spect.Net.SpectrumEmu.Devices.Floppy
{
    /// <summary>
    /// This class implements a virtual floppy file
    /// </summary>
    public class VirtualFloppyFile
    {
        // ReSharper disable once InconsistentNaming
        public static readonly byte[] HEADER = { (byte)'V', (byte)'F', (byte)'D' };

        /// <summary>
        /// Sector size in bytes
        /// </summary>
        public const int SECTOR_SIZE = 512;

        /// <summary>
        /// File header size
        /// </summary>
        public const int HEADER_SIZE = 8;

        /// <summary>
        /// Disk format
        /// </summary>
        public FloppyFormat DiskFormat { get; }

        /// <summary>
        /// The name of the floppy file with full path
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Is the ployy write protected?
        /// </summary>
        public bool IsWriteProtected { get; }

        /// <summary>
        /// Signs if the floppy is double-sided
        /// </summary>
        public bool IsDoubleSided { get; }

        /// <summary>
        /// Gets the number of tracks
        /// </summary>
        public byte Tracks { get; }

        /// <summary>
        /// Gest the number of sectors per track
        /// </summary>
        public byte SectorsPerTrack { get; }

        /// <summary>
        /// First sector index of the current format
        /// </summary>
        public byte FirstSectorIndex { get;  }

        /// <summary>
        /// Format specifier
        /// </summary>
        public List<byte> FormatSpec { get; }

        /// <summary>
        /// Checks if the current drive is Spectrum compatible
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSpectrumVmCompatible()
        {
            return true;
        }

        /// <summary>
        /// We do not allow direct instantiation
        /// </summary>
        private VirtualFloppyFile(string filename, bool isWriteProtected, 
            IReadOnlyList<byte> formatSpec, byte firsSectorIndex)
        {
            Filename = filename;
            FormatSpec = new List<byte>(formatSpec);
            IsWriteProtected = isWriteProtected;
            IsDoubleSided = formatSpec[1] != 0;
            Tracks = formatSpec[2];
            SectorsPerTrack = formatSpec[3];
            FirstSectorIndex = firsSectorIndex;
            switch (formatSpec[0])
            {
                case 0x00:
                    DiskFormat = FloppyFormat.SpectrumP3;
                    break;
                case 0x01:
                    DiskFormat = FloppyFormat.CpcSystem;
                    break;
                case 0x02:
                    DiskFormat = FloppyFormat.CpcData;
                    break;
                default:
                    DiskFormat = FloppyFormat.Pcw;
                    break;
            }
        }

        /// <summary>
        /// Creates a Spectrum compatible floppy file with the specified name
        /// </summary>
        /// <param name="filename">File name with full pass</param>
        /// <param name="format">Format to create</param>
        /// <returns>The newly created floppy file</returns>
        public static VirtualFloppyFile CreateSpectrumFloppyFile(string filename, FloppyFormat format = FloppyFormat.SpectrumP3)
        {
            var dir = Path.GetDirectoryName(filename);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var writer = new BinaryWriter(File.Create(filename)))
            {
                if (!s_FormatHeaders.TryGetValue(format, out var formatDesc))
                {
                    formatDesc = s_DefaultFormatDescriptor;
                }
                var formatBytes = formatDesc.Format;
                var floppy = new VirtualFloppyFile(filename, false, formatBytes, formatDesc.SectorIndex);
                writer.Write(HEADER);
                writer.Write(floppy.IsWriteProtected ? (byte)0x01 : (byte)0x00);
                writer.Write(floppy.IsDoubleSided ? (byte)0x01 : (byte)0x00);
                writer.Write(floppy.Tracks);
                writer.Write(floppy.SectorsPerTrack);
                writer.Write(floppy.FirstSectorIndex);
                var sectorData = new byte[512];
                for (var i = 0; i < sectorData.Length; i++)
                {
                    sectorData[i] = 0xE5;
                }
                for (var i = 0; i < (floppy.IsDoubleSided ? 2 : 1) * floppy.Tracks * floppy.SectorsPerTrack; i++)
                {
                    writer.Write(sectorData);
                }
                writer.Seek(HEADER_SIZE, SeekOrigin.Begin);
                writer.Write(formatBytes);
                return floppy;
            }
        }

        /// <summary>
        /// Creates a Spectrum compatible floppy file with the specified name
        /// </summary>
        /// <param name="filename">File name with full pass</param>
        /// <returns>The opened floppy file</returns>
        public static VirtualFloppyFile OpenFloppyFile(string filename)
        {
            using (var reader = new BinaryReader(File.OpenRead(filename)))
            {
                var header = reader.ReadBytes(HEADER.Length);
                if (!header.SequenceEqual(HEADER))
                {
                    throw new InvalidOperationException("Invalid floppy file header");
                }

                var isWriteProtected = reader.ReadByte() != 0x00;
                var isDoubleSided  = reader.ReadByte() != 0x00;
                var tracks = reader.ReadByte();
                var sectorsPerTrack = reader.ReadByte();
                var firstSector = reader.ReadByte();
                var lengthRead = 0;
                for (var i = 0; i < (isDoubleSided ? 2 : 1) * tracks * sectorsPerTrack; i++)
                {
                    var sectorData = reader.ReadBytes(512);
                    lengthRead += sectorData.Length;
                    if (sectorData.Length < 512)
                    {
                        throw new InvalidOperationException(
                            $"Floppy file is shorter then expected, its size is {lengthRead} bytes.");
                    }
                }
                reader.BaseStream.Seek(HEADER_SIZE, SeekOrigin.Begin);
                var formatSpec = reader.ReadBytes(10);
                var floppy = new VirtualFloppyFile(filename, isWriteProtected, formatSpec, firstSector);
                if (!floppy.IsSpectrumVmCompatible())
                {
                    throw new InvalidOperationException("Floppy file is not Spectrum compatible");
                }
                return floppy;
            }
        }

        /// <summary>
        /// Opens a Spectrum compatible floppy file with the specified name, or
        /// creates if that file does not exist
        /// </summary>
        /// <param name="filename">File name with full pass</param>
        /// <returns>The opened floppy file</returns>
        public static VirtualFloppyFile OpenOrCreateFloppyFile(string filename)
        {
            return File.Exists(filename)
                ? OpenFloppyFile(filename)
                : CreateSpectrumFloppyFile(filename);
        }

        /// <summary>
        /// Sets the specified write protection mode of the disk
        /// </summary>
        /// <param name="filename">Disk file name</param>
        /// <param name="isWriteProtected">Write protection flag</param>
        public static void SetWriteProtection(string filename, bool isWriteProtected)
        {
            // --- This operation allows to check the file for corruption
            OpenFloppyFile(filename);

            using (var writer = new BinaryWriter(File.OpenWrite(filename)))
            {
                writer.BaseStream.Seek(HEADER.Length, SeekOrigin.Begin);
                writer.Write(isWriteProtected ? (byte) 0x01 : (byte) 0x00);
            }
        }

        /// <summary>
        /// Check if the specified file has write protection or not
        /// </summary>
        /// <param name="filename">Disk file name</param>
        /// <returns>True, if the disk is write protected</returns>
        public static bool CheckWriteProtection(string filename)
        {
            try
            {
                using (var reader = new BinaryReader(File.OpenRead(filename)))
                {
                    reader.BaseStream.Seek(HEADER.Length, SeekOrigin.Begin);
                    return reader.ReadByte() != 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the data to the file
        /// </summary>
        /// <param name="head">Head parameter (0 or 1)</param>
        /// <param name="track">Track parameter (starts from 0)</param>
        /// <param name="sector">Sector parameter (starts from 1)</param>
        /// <param name="data">Data to write to</param>
        public void WriteData(int head, int track, int sector, byte[] data)
        {
            if (sector > 9)
            {
                sector = sector - FirstSectorIndex + 1;
            }

            CheckPositionParameters(head, track, sector);
            if (data.Length == 0)
            {
                throw new ArgumentException("Data must be at least one byte", nameof(data));
            }

            if (data.Length > SECTOR_SIZE)
            {
                throw new ArgumentException($"Data cannot be longer than {SECTOR_SIZE} bytes", nameof(data));
            }

            using (var writer = new BinaryWriter(File.OpenWrite(Filename)))
            {
                writer.Seek(CalculateSectorPosition(head, track, sector), SeekOrigin.Begin);
                writer.Write(data);
            }
        }

        /// <summary>
        /// Reads data from the file
        /// </summary>
        /// <param name="head">Head parameter (0 or 1)</param>
        /// <param name="track">Track parameter (starts from 0)</param>
        /// <param name="sector">Sector parameter (starts from 1)</param>
        /// <param name="length">Number of bytes to read</param>
        public byte[] ReadData(int head, int track, int sector, int length)
        {
            if (sector > 9)
            {
                sector = sector - FirstSectorIndex + 1;
            }
            CheckPositionParameters(head, track, sector);
            if (length == 0)
            {
                throw new ArgumentException("Data must be at least one byte", nameof(length));
            }
            if (length > SECTOR_SIZE)
            {
                throw new ArgumentException($"Data cannot be longer than {SECTOR_SIZE} bytes", nameof(length));
            }
            using (var reader = new BinaryReader(File.OpenRead(Filename)))
            {
                reader.BaseStream.Seek(CalculateSectorPosition(head, track, sector), SeekOrigin.Begin);
                return reader.ReadBytes(length);
            }
        }

        /// <summary>
        /// Checks the range of the specified position parameters
        /// </summary>
        private void CheckPositionParameters(int head, int track, int sector)
        {
            if (head != 0 && head != 1)
            {
                throw new ArgumentException("Head must be 0 or 1", nameof(head));
            }
            if (track < 0 || track >= Tracks)
            {
                throw new ArgumentException($"Track must be 0 and {Tracks - 1}", nameof(track));
            }
            if (sector < 1 || sector > SectorsPerTrack)
            {
                throw new ArgumentException($"Sector must be 1 and {SectorsPerTrack}", nameof(sector));
            }
        }

        /// <summary>
        /// Calculates the seek position of a specified sector
        /// </summary>
        private int CalculateSectorPosition(int head, int track, int sector)
        {
            return
                HEADER_SIZE                                     // Header offset
                + head * Tracks * SectorsPerTrack * SECTOR_SIZE // Side offset
                + track * SectorsPerTrack * SECTOR_SIZE         // Track offset
                + (sector - 1) * SECTOR_SIZE;                   // Sector offset
        }

        /// <summary>
        /// Default format descriptor, if none found by ID
        /// </summary>
        private static readonly (byte[] Format, byte SectorIndex) s_DefaultFormatDescriptor =
            (new byte[] {0x00, 0x00, 0x28, 0x09, 0x02, 0x01, 0x03, 0x02, 0x2a, 0x52}, 0x01);

        /// <summary>
        /// The available format descriptors
        /// </summary>
        private static readonly Dictionary<FloppyFormat, (byte[] Format, byte SectorIndex)> s_FormatHeaders = 
            new Dictionary<FloppyFormat, (byte[], byte)>
        {
            {
                FloppyFormat.SpectrumP3, (new byte[] { 0x00, 0x00, 0x28, 0x09, 0x02, 0x01, 0x03, 0x02, 0x2a, 0x52}, 0x01)
            },
            {
                FloppyFormat.CpcSystem, (new byte[] { 0x01, 0x00, 0x28, 0x09, 0x02, 0x02, 0x03, 0x02, 0x2a, 0x52}, 0x41)
            },
            {
                FloppyFormat.CpcData, (new byte[] { 0x02, 0x00, 0x28, 0x09, 0x02, 0x00, 0x03, 0x02, 0x2a, 0x52}, 0xC1)
            },
            {
                FloppyFormat.Pcw, (new byte[] { 0x03, 0x81, 0x50, 0x09, 0x02, 0x01, 0x04, 0x04, 0x2a, 0x52}, 0x01)
            }
        };
    }
}