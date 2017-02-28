using System;

namespace Spect.Net.Z80Tests.Mvvm.Attributes
{
    /// <summary>
    /// This attribute tells that this view model is asynchronously initialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AsyncInitAttribute : Attribute
    {
        public bool Value { get; }

        public AsyncInitAttribute(bool value = true)
        {
            Value = value;
        }
    }
}