using System;

namespace Spect.Net.Ide.Mvvm.Attributes
{
    /// <summary>
    /// This attribute represents the resource to be displayed in the view
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewResourceAttribute : Attribute
    {
        public string Resource { get; }

        public ViewResourceAttribute(string resource)
        {
            Resource = resource;
        }
    }
}