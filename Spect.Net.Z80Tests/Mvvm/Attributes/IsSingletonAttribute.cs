using System;

namespace Spect.Net.Z80Tests.Mvvm.Attributes
{
    /// <summary>
    /// This attribute tells whether its view model is a singleton.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IsSingletonAttribute : Attribute
    {
        public bool Value { get; }

        public IsSingletonAttribute(bool value = true)
        {
            Value = value;
        }
    }
}