using System;

namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class is intended to be the base class of a Next feature control register
    /// </summary>
    public abstract class FeatureControlRegisterBase
    {
        /// <summary>
        /// Register ID
        /// </summary>
        public byte Id { get; }

        /// <summary>
        /// Is the register readable?
        /// </summary>
        public bool CanRead { get; }

        /// <summary>
        /// Is the register writeable?
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        /// Gest the last value for testing/production purposes
        /// </summary>
        public byte LastValue { get; protected set; }

        /// <summary>
        /// This event is raised when the value of the register changes.
        /// </summary>
        public event EventHandler<RegisterSetEventArgs> RegisterValueSet;
        
        /// <summary>
        /// Initializes a new instance of the feature control register.
        /// </summary>
        /// <param name="id">Register index/ID</param>
        /// <param name="canRead">Is readable?</param>
        /// <param name="canWrite">Is writeable</param>
        protected FeatureControlRegisterBase(byte id, bool canRead = true, bool canWrite = true)
        {
            Id = id;
            CanRead = canRead;
            CanWrite = canWrite;
        }

        /// <summary>
        /// Writes the register value through a NEXTREG CPU operation or a direct 0x253B
        /// port write, provided, write is allowed.
        /// </summary>
        /// <param name="value">Value to write into the register</param>
        public void Write(byte value)
        {
            if (CanWrite)
            {
                LastValue = value;
                RegisterValueSet?.Invoke(this, new RegisterSetEventArgs(value));
            }
        }

        /// <summary>
        /// Reads the register value through a direct $253B read operation, provided, 
        /// read is allowed.
        /// </summary>
        /// <returns>Value of the register, if read is allowed; otherwise, 0xFF</returns>
        public byte Read()
        {
            return CanRead ? LastValue : (byte)0xFF;
        }

        /// <summary>
        /// Writes the value of the register through the Next emulator software
        /// </summary>
        /// <param name="value">Value to write</param>
        public void Set(byte value)
        {
            LastValue = value;
        }
    }

    /// <summary>
    /// Arguments for the register changes event
    /// </summary>
    public class RegisterSetEventArgs : EventArgs
    {
        /// <summary>
        /// Register value set
        /// </summary>
        public byte Value { get; }

        public RegisterSetEventArgs(byte value)
        {
            Value = value;
        }
    }
}