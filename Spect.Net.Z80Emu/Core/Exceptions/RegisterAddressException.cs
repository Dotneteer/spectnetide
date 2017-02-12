namespace Spect.Net.Z80Emu.Core.Exceptions
{
    /// <summary>
    /// A register index has been used that is out of the index range
    /// </summary>
    public class RegisterAddressException : Z80EmuException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> 
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public RegisterAddressException(string message) : base(message)
        {
        }
    }
}