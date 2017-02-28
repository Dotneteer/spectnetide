using System;

namespace Spect.Net.Z80Tests.Mvvm.Attributes
{
    /// <summary>
    /// This attribute tells whether its view model should be
    /// excluded from navigation history.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeFromNavigationHistoryAttribute : Attribute
    {
        public bool Value { get; }

        public ExcludeFromNavigationHistoryAttribute(bool value = true)
        {
            Value = value;
        }
    }
}