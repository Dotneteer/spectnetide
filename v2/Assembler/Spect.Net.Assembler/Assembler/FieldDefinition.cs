namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class defines a field of a structure
    /// </summary>
    public class FieldDefinition: IHasUsageInfo
    {
        /// <summary>
        /// The field offset within the structure
        /// </summary>
        public ushort Offset { get; }

        /// <summary>
        /// Indicates if the field has been referenced or not
        /// </summary>
        public bool IsUsed { get; set; }

        public FieldDefinition(ushort offset)
        {
            Offset = offset;
        }
    }
}