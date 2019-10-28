using System;

namespace Spect.Net.VsPackage.VsxLibrary.Output
{
    /// <summary>
    /// This attribute declares the initial visibility of the output window pane.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InitiallyVisibleAttribute: Attribute
    {
        /// <summary>
        /// Gets the value of this attribute.
        /// </summary>
        public bool Value { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public InitiallyVisibleAttribute(bool value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// This attribute declares if the output window pane should be cleared when the current solution 
    /// is closed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ClearWithSolutionAttribute : Attribute
    {
        /// <summary>
        /// Gets the value of this attribute.
        /// </summary>
        public bool Value { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Attribute" /> class.</summary>
        public ClearWithSolutionAttribute(bool value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// This attribute declares if output window pane should be activated automatically after the 
    /// first write operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AutoActivateAttribute : Attribute
    {
        /// <summary>
        /// Gets the value of this attribute.
        /// </summary>
        public bool Value { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Attribute" /> class.</summary>
        public AutoActivateAttribute(bool value)
        {
            Value = value;
        }
    }
}