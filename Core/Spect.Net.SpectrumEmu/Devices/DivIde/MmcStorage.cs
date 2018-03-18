using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.DivIde
{
    /// <summary>
    /// This class represents the MMC storage that is persisted into a file
    /// </summary>
    /// <remarks>
    /// The MMC Storage is a binary file with theis structure:
    /// ------------------------------------------------------
    /// Header:      16 bytes
    /// Map:         16368 bytes
    /// Data Blocks: 64 Kbytes each
    /// 
    /// Header structure
    /// ----------------
    ///   Prefix:   4 bytes, "MMC_"
    ///   MBlocks:  2 bytes, Maximum number of blocks (card size)
    ///   CBlocks:  2 bytes, Current number of blocks
    ///   Map size: 2 bytes, currently 16368, but in the future, it may change
    ///   Reserved: 6 bytes, Reserved for future use
    /// 
    /// Map
    /// ---
    /// The 16368 bytes is 8144 words, so they represent up to 8144 * 64 KByte of
    /// storage. The map word with zero index represents the first 64 Kbyte, index 1
    /// the second 64 Kbyte, and so on. Each map word specifies the index of physical
    /// block that contains the corresponding 64 Kbyte of the storage. This structure
    /// allows a storage file that continuously increases as the storage is used.
    /// The unmapped entries contain 0xFFFF. If there's a reference to a non-existing
    /// block, all the 64 Kbyte data is taken into account as if zeros were written
    /// to that block.
    /// 
    /// Data Blocks
    /// -----------
    /// The bytes are continuously mapped from index 0x0000 to 0xFFFF.
    /// 
    /// </remarks>
    public class MmcStorage
    {
        /// <summary>
        /// The number of entries in the map
        /// </summary>
        public const int MAP_SIZE = 8144;

        private readonly byte[] _reserved = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly ushort[] _blockMap = new ushort[MAP_SIZE];

        /// <summary>
        ///  The file prefix
        /// </summary>
        public uint Id => ((byte) 'M' << 24) | ((byte) 'M' << 16) | ((byte) 'C' << 8) | (byte) '_';

        /// <summary>
        /// The maximum number of blocks in this card
        /// </summary>
        public ushort MBlocks { get; private set; }

        /// <summary>
        /// The current number of blocks within the card
        /// </summary>
        public ushort CBlocks { get; private set; }

        /// <summary>
        /// The number of map entries (words)
        /// </summary>
        public ushort MapSize { get; private set; }

        /// <summary>
        /// Reserved for future use
        /// </summary>
        public ReadOnlyCollection<byte> Reserved => new ReadOnlyCollection<byte>(_reserved);

        /// <summary>
        /// The map of blocks
        /// </summary>
        public ReadOnlyCollection<ushort> Map => new ReadOnlyCollection<ushort>(_blockMap);

        /// <summary>
        /// The file that stores the data of the MMC storage
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The size of the card in MBytes
        /// </summary>
        public int SizeInMb { get; }

        /// <summary>
        /// Initializes an MMC card that uses the specified file as storage, with
        /// the given size (in MBytes)
        /// </summary>
        /// <param name="filename">Storage file name</param>
        /// <param name="sizeInMb">Card size in MBytes</param>
        private MmcStorage(string filename, int sizeInMb)
        {
            if (sizeInMb < 64) sizeInMb = 32;
            else if (sizeInMb >= 64 && sizeInMb < 128) sizeInMb = 64;
            else if (sizeInMb >= 128 && sizeInMb < 256) sizeInMb = 128;
            else if (sizeInMb >= 256) sizeInMb = 256;
            SizeInMb = sizeInMb;
            Filename = filename;
            MBlocks = (ushort)(sizeInMb * 1024 / 64);
            CBlocks = 0;
            MapSize = MAP_SIZE;
        }

        /// <summary>
        /// Factory method to create an MMC storage object
        /// </summary>
        /// <param name="filename">Storage file name</param>
        /// <param name="sizeInMb">Card size in MBytes</param>
        /// <returns>Newly created storage object</returns>
        public static MmcStorage Create(string filename, int sizeInMb)
            => new MmcStorage(filename, sizeInMb);

        #region Helpers

        private void CreateOrOpenFile()
        {
            if (File.Exists(Filename))
            {
                // --- Open the file and read its header/map information
                using (var br = new BinaryReader(File.OpenRead(Filename)))
                {
                    // --- Get the header
                    var prefix = br.ReadUInt32();
                    if (prefix != Id)
                    {
                        throw new InvalidOperationException("Invalid MMC file prefix");
                    }

                    MBlocks = br.ReadUInt16();
                    CBlocks = br.ReadUInt16();
                    MapSize = br.ReadUInt16();
                    if (MapSize != _blockMap.Length)
                    {
                        throw new InvalidOperationException($"Map sizes different than {MAP_SIZE} are not supported yet.");
                    }
                    br.ReadBytes(_reserved.Length);

                    // --- Get the maps
                    for (var i = 0; i < _blockMap.Length; i++)
                    {
                        _blockMap[i] = br.ReadUInt16();
                    }
                }
                return;
            }

            // --- The file does not exist, create it
            using (var bw = new BinaryWriter(File.Create(Filename)))
            {
                bw.Write(Id);
                bw.Write(MBlocks);
                bw.Write(CBlocks);
                bw.Write(MapSize);
                for (var i = 0; i < MapSize; i++)
                {
                    bw.Write((ushort)0xFFFF);
                }
            }
        }

        #endregion
    }
}