using System;
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
        public static readonly byte[] HEADER = { (byte)'V', (byte)'F', (byte)'D', (byte)'D', (byte)'F' };

        /// <summary>
        /// Sector size in bytes
        /// </summary>
        public const int SECTOR_SIZE = 512;

        /// <summary>
        /// File header size
        /// </summary>
        public const int HEADER_SIZE = 8;

        /// <summary>
        /// The name of the floppy file with full path
        /// </summary>
        public string Filename { get; }
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
        /// Checks if the current drive is Spectrum compatible
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSpectrumVmCompatible()
        {
            return IsDoubleSided && Tracks == 40 && SectorsPerTrack == 9;
        }

        /// <summary>
        /// We do not allow direct instantiation
        /// </summary>
        private VirtualFloppyFile(string filename, bool isDoubleSided, byte tracks, byte sectorsPerTrack)
        {
            Filename = filename;
            IsDoubleSided = isDoubleSided;
            Tracks = tracks;
            SectorsPerTrack = sectorsPerTrack;
        }

        /// <summary>
        /// Creates a Spectrum compatible floppy file with the specified name
        /// </summary>
        /// <param name="filename">File name with full pass</param>
        /// <returns>The newly created floppy file</returns>
        public static VirtualFloppyFile CreateSpectrumFloppyFile(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var writer = new BinaryWriter(File.Create(filename)))
            {
                var floppy = new VirtualFloppyFile(filename, true, 40, 9);
                writer.Write(HEADER);
                writer.Write(floppy.IsDoubleSided ? (byte)0x01 : (byte)0x00);
                writer.Write(floppy.Tracks);
                writer.Write(floppy.SectorsPerTrack);
                var sectorData = new byte[512];
                for (var i = 0; i < (floppy.IsDoubleSided ? 2 : 1) * floppy.Tracks * floppy.SectorsPerTrack; i++)
                {
                    writer.Write(sectorData);
                }
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
                var isDoubleSided  = reader.ReadByte() != 0x00;
                var tracks = reader.ReadByte();
                var sectorsPerTrack = reader.ReadByte();
                var floppy = new VirtualFloppyFile(filename, isDoubleSided, tracks, sectorsPerTrack);
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
        /// Writes the data to the file
        /// </summary>
        /// <param name="head">Head parameter (0 or 1)</param>
        /// <param name="track">Track parameter (starts from 0)</param>
        /// <param name="sector">Sector parameter (starts from 1)</param>
        /// <param name="data">Data to write to</param>
        public void WriteData(int head, int track, int sector, byte[] data)
        {
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
                throw new ArgumentException($"Track must be 1 and {SectorsPerTrack}", nameof(sector));
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
    }
}