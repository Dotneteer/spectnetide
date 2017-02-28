using System;

namespace Spect.Net.Z80Tests.Mvvm.Attributes
{
    /// <summary>
    /// This attribute represents the title to be displayed in the view
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewTitleAttribute : Attribute
    {
        public string Title { get; }

        public ViewTitleAttribute(string title)
        {
            Title = title;
        }
    }
}