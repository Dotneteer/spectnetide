using System;
using System.Globalization;

// ReSharper disable SwitchStatementMissingSomeCases

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the value of an evaluated expression
    /// </summary>
    public class ExpressionValue
    {
        /// <summary>
        /// Used in case of expression evaluation errors
        /// </summary>
        public static ExpressionValue Error = new ExpressionValue();

        /// <summary>
        /// Represents non-evaluated value
        /// </summary>
        public static ExpressionValue NonEvaluated = new ExpressionValue(ExpressionValueType.NonEvaluated);

        private readonly long _numValue;
        private readonly double _realValue;
        private readonly string _stringValue;

        /// <summary>
        /// The type of the expression value
        /// </summary>
        public ExpressionValueType Type { get; }

        /// <summary>
        /// Checks if the value of this expression is valid
        /// </summary>
        public bool IsValid => this != NonEvaluated && this != Error;

        /// <summary>
        /// Checks if the expression is evaluated
        /// </summary>
        public bool IsNonEvaluated => this == NonEvaluated; 

        /// <summary>
        /// Initializes a new instance of the class with the Error value.
        /// </summary>
        public ExpressionValue()
        {
            Type = ExpressionValueType.Error;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ExpressionValue(ExpressionValueType type)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified Boolean value.
        /// </summary>
        /// <param name="value">Boolean value</param>
        public ExpressionValue(bool value)
        {
            Type = ExpressionValueType.Bool;
            _numValue = value ? 1 : 0;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified Number value.
        /// </summary>
        /// <param name="value">Number value</param>
        public ExpressionValue(long value)
        {
            Type = ExpressionValueType.Integer;
            _numValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified Real value.
        /// </summary>
        /// <param name="value">Real value</param>
        public ExpressionValue(double value)
        {
            Type = ExpressionValueType.Real;
            _realValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified String value.
        /// </summary>
        /// <param name="value">String value</param>
        public ExpressionValue(string value)
        {
            Type = ExpressionValueType.String;
            _stringValue = value;
        }

        /// <summary>
        /// Shortcut property to get the 16-bit value behind the expression
        /// </summary>
        public ushort Value => AsWord();

        /// <summary>
        /// Gets the expression's value as a Boolean
        /// </summary>
        /// <returns>Bool representation of the value</returns>
        public bool AsBool()
        {
            switch (Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    return _numValue != 0;
                case ExpressionValueType.Real:
                    return Math.Abs(_realValue) > Double.Epsilon;
                case ExpressionValueType.String:
                    return !string.IsNullOrEmpty(_stringValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the expression's value as a Number
        /// </summary>
        /// <returns>Number representation of the value</returns>
        public long AsLong()
        {
            switch (Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    return _numValue;
                case ExpressionValueType.Real:
                    return (long)_realValue;
                case ExpressionValueType.String:
                    if (long.TryParse(_stringValue, out var result))
                    {
                        return result;
                    }
                    throw new InvalidOperationException("Cannot convert string to an integer number");
                default:
                    return 0L;
            }
        }

        /// <summary>
        /// Gets the expression's value as a Number
        /// </summary>
        /// <returns>Number representation of the value</returns>
        public double AsReal()
        {
            switch (Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    return _numValue;
                case ExpressionValueType.Real:
                    return _realValue;
                case ExpressionValueType.String:
                    if (double.TryParse(_stringValue, out var result))
                    {
                        return result;
                    }
                    throw new InvalidOperationException("Cannot convert string to a real number");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the expression's value as a Number
        /// </summary>
        /// <returns>Number representation of the value</returns>
        public string AsString()
        {
            switch (Type)
            {
                case ExpressionValueType.Bool:
                    return _numValue == 1 ? "true" : "false";
                case ExpressionValueType.Integer:
                    return _numValue.ToString();
                case ExpressionValueType.Real:
                    return _realValue.ToString(CultureInfo.InvariantCulture);
                case ExpressionValueType.String:
                    return _stringValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the expression's value as a Word
        /// </summary>
        /// <returns>Word representation of the value</returns>
        public ushort AsWord() => (ushort)AsLong();

        /// <summary>
        /// Gets the expression's value as a Byte
        /// </summary>
        /// <returns>Byte representation of the value</returns>
        public byte AsByte() => (byte)AsLong();

        /// <summary>
        /// Implicit operator to ushort
        /// </summary>
        /// <param name="value">Value to convert</param>
        public static implicit operator ushort(ExpressionValue value) => value.Value;
    }
}