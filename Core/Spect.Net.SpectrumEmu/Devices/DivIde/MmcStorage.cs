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
    ///   MBlocks:  4 bytes, Maximum number of blocks (card size)
    ///   CBlocks:  4 bytes, Current number of blocks
    ///   Map size: 4 bytes, currently 16368, but in the future, it may change
    /// 
    /// Map
    /// ---
    /// The 16368 bytes is 8184 words, so they represent up to 8144 * 64 KByte of
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
        public const int MAP_SIZE = 8184;

        private readonly ushort[] _blockMap = new ushort[MAP_SIZE];
        private byte[] _cachedData;

        /// <summary>
        ///  The file prefix
        /// </summary>
        public uint Id => ((byte) 'M') | ((byte) 'M' << 8) | ((byte) 'C' << 16) | (byte) '_' << 24;

        /// <summary>
        /// The maximum number of blocks in this card
        /// </summary>
        public int MBlocks { get; private set; }

        /// <summary>
        /// The current number of blocks within the card
        /// </summary>
        public int CBlocks { get; private set; }

        /// <summary>
        /// The number of map entries (words)
        /// </summary>
        public int MapSize { get; private set; }

        /// <summary>
        /// The block currently stored in the cache
        /// </summary>
        public int CachedBlock { get; private set; }

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
            CreateOrOpenFile();
            CachedBlock = -1;
        }

        /// <summary>
        /// Factory method to create an MMC storage object
        /// </summary>
        /// <param name="filename">Storage file name</param>
        /// <param name="sizeInMb">Card size in MBytes</param>
        /// <returns>Newly created storage object</returns>
        public static MmcStorage Create(string filename, int sizeInMb)
            => new MmcStorage(filename, sizeInMb);

        /// <summary>
        /// Writes the specified byte to the given address of the storage
        /// </summary>
        /// <param name="address">Address to write the data to</param>
        /// <param name="data">Data byte to write</param>
        public void WriteData(int address, byte data)
        {
            var blockNo = address >> 16;
            if (blockNo >= MBlocks)
            {
                throw new InvalidOperationException(
                    $"Address {address} exceeds the end of the storage.");
            }
            ReadBlockIntoCache(blockNo);
            var position = address & 0xFFFF;
            _cachedData[position] = data;
            WriteOutCache();
        }

        /// <summary>
        /// Writes the specified byte array to the given address of the storage
        /// </summary>
        /// <param name="address">Address to write the data to</param>
        /// <param name="data">Byte array to write</param>
        public void WriteData(int address, byte[] data)
        {
            var endBlockNo = (address + data.Length - 1) >> 16;
            if (endBlockNo >= MBlocks)
            {
                throw new InvalidOperationException(
                    $"Block starting at {address} with length of {data.Length} exceeds the end of the storage.");
            }

            var lastBlockNo = -1;
            for (var i = 0; i < data.Length; i++)
            {
                var blockNo = (address + i) >> 16;
                var position = (address + i) & 0xFFFF;
                if (blockNo != lastBlockNo)
                {
                    WriteOutCache();
                    ReadBlockIntoCache(blockNo);
                    _cachedData[position] = data[i];
                    lastBlockNo = blockNo;
                }
                else
                {
                    _cachedData[position] = data[i];
                }
            }
            WriteOutCache();
        }

        /// <summary>
        /// Reads the data byte from the specified address
        /// </summary>
        /// <param name="address">Address to read the data from</param>
        /// <returns>Data byte read from MMC</returns>
        public byte ReadData(int address)
        {
            var blockNo = address >> 16;
            if (blockNo >= MBlocks)
            {
                throw new InvalidOperationException(
                    $"Address {address} exceeds the end of the storage.");
            }
            ReadBlockIntoCache(blockNo);
            return _cachedData[address & 0xFFFF];
        }

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

                    MBlocks = br.ReadInt32();
                    CBlocks = br.ReadInt32();
                    MapSize = br.ReadInt32();
                    if (MapSize != _blockMap.Length)
                    {
                        throw new InvalidOperationException($"Map sizes different than {MAP_SIZE} are not supported yet.");
                    }

                    // --- Get the maps
                    for (var i = 0; i < _blockMap.Length; i++)
                    {
                        _blockMap[i] = br.ReadUInt16();
                    }
                }
                return;
            }

            // --- Take care that the containing folder exists
            var folder = Path.GetDirectoryName(Filename) ?? string.Empty;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
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
                    _blockMap[i] = 0xFFFF;
                }
            }
        }

        /// <summary>
        /// Reads the specified block into the cache. If the
        /// block does not exists, this method creates it.
        /// </summary>
        /// <param name="blockNo">Block number to read into the cache</param>
        private void ReadBlockIntoCache(int blockNo)
        {
            // --- Maybe, the block is already in the cache
            if (blockNo == CachedBlock) return;

            // --- Just to be absolutely sure
            if (blockNo >= _blockMap.Length)
            {
                throw new InvalidOperationException(
                    $"Block index {blockNo} is greater than the maximum block size {_blockMap.Length}");
            }

            if (_blockMap[blockNo] == 0xFFFF)
            {
                // --- The specified block does not exist in the storage file,
                // --- Let's create it
                using (var bw = new BinaryWriter(File.OpenWrite(Filename)))
                {
                    // --- Write 8192 long zeros to the end
                    bw.Seek(0, SeekOrigin.End);
                    for (var i = 0; i < 0x2000; i++)
                    {
                        bw.Write(0L);
                    }
                    CBlocks++;
                    bw.Seek(4 + 4, SeekOrigin.Begin);
                    bw.Write(CBlocks);
                    bw.Seek(16 + 2 * blockNo, SeekOrigin.Begin);
                    bw.Write((ushort)(CBlocks - 1));
                }

                // --- Create an empty cache and map the newly created block
                _cachedData = new byte[0x10000];
                _blockMap[blockNo] = (ushort)(CBlocks - 1);
                CachedBlock = blockNo;
                return;
            }

            // --- The specified block should exist in the file, read it
            using (var br = new BinaryReader(File.OpenRead(Filename)))
            {
                br.BaseStream.Seek(0x4000 + 0x10000 * blockNo, SeekOrigin.Begin);
                _cachedData = br.ReadBytes(0x10000);
                CachedBlock = blockNo;
            }
        }

        /// <summary>
        /// Write out the entire cache to the storage file
        /// </summary>
        private void WriteOutCache()
        {
            if (CachedBlock < 0) return;
            using (var bw = new BinaryWriter(File.OpenWrite(Filename)))
            {
                var physBlock = _blockMap[CachedBlock];
                bw.Seek(0x4000 + 0x10000 * physBlock, SeekOrigin.Begin);
                bw.Write(_cachedData);
            }
        }

        #endregion
    }
}