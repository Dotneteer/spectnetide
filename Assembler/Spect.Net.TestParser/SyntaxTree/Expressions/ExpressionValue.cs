using System;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the value of an evaluated expression
    /// </summary>
    public class ExpressionValue
    {
        /// <summary>
        /// Represents the Zero value, used in case of expression evaluation errors
        /// </summary>
        public static ExpressionValue Error = new ExpressionValue();

        private readonly ushort _numValue;
        private readonly byte[] _arrayValue;

        /// <summary>
        /// The type of the expression value
        /// </summary>
        public ExpressionValueType Type { get; }

        /// <summary>
        /// Initializes a new instance of the class with the Error value.
        /// </summary>
        public ExpressionValue()
        {
            Type = ExpressionValueType.Error;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified Boolean value.
        /// </summary>
        /// <param name="value">Boolean value</param>
        public ExpressionValue(bool value)
        {
            Type = ExpressionValueType.Bool;
            _numValue = value ? (ushort)1 : (ushort)0;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified byte value.
        /// </summary>
        /// <param name="value">Byte value</param>
        public ExpressionValue(byte value)
        {
            Type = ExpressionValueType.Byte;
            _numValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified Word value.
        /// </summary>
        /// <param name="value">Word value</param>
        public ExpressionValue(ushort value)
        {
            Type = ExpressionValueType.Word;
            _numValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified ByteArray value.
        /// </summary>
        /// <param name="value">Word value</param>
        public ExpressionValue(byte[] value)
        {
            Type = ExpressionValueType.ByteArray;
            _numValue = 0;
            _arrayValue = value;
        }

        /// <summary>
        /// Gets the expression's value as a Boolean
        /// </summary>
        /// <returns>Bool representation of the value</returns>
        public bool AsBool() => Type == ExpressionValueType.ByteArray
            ? throw new InvalidOperationException("ByteArray cannot be converted to bool")
            : _numValue != 0;

        /// <summary>
        /// Gets the expression's value as a Byte
        /// </summary>
        /// <returns>Byte representation of the value</returns>
        public byte AsByte() => Type == ExpressionValueType.ByteArray
            ? throw new InvalidOperationException("ByteArray cannot be converted to byte")
            : (byte)_numValue;

        /// <summary>
        /// Gets the expression's value as a Word
        /// </summary>
        /// <returns>Byte representation of the value</returns>
        public ushort AsWord() => Type == ExpressionValueType.ByteArray
            ? throw new InvalidOperationException("ByteArray cannot be converted to word")
            : _numValue;

        /// <summary>
        /// Gets the expression's value as a ByteArray
        /// </summary>
        /// <returns>Byte representation of the value</returns>
        public byte[] AsByteArray() => Type == ExpressionValueType.ByteArray
            ? _arrayValue
            : new [] { (byte)_numValue };
    }
}