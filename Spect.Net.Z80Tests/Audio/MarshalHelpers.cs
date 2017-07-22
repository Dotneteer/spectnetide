using System;
using System.Runtime.InteropServices;

namespace Spect.Net.Z80Tests.Audio
{
    /// <summary>
    /// Support for Marshal Methods in both UWP and .NET Framework
    /// </summary>
    public static class MarshalHelpers
    {
        /// <summary>
        /// SizeOf a structure
        /// </summary>
        public static int SizeOf<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }

        /// <summary>
        /// Offset of a field in a structure
        /// </summary>
        public static IntPtr OffsetOf<T>(string fieldName)
        {
            return Marshal.OffsetOf(typeof(T), fieldName);
        }

        /// <summary>
        /// Pointer to Structure
        /// </summary>
        public static T PtrToStructure<T>(IntPtr pointer)
        {
            return (T)Marshal.PtrToStructure(pointer, typeof(T));
        }
    }
}