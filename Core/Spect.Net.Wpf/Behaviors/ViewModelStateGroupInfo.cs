using System.Reflection;
using System.Windows;

namespace Spect.Net.Wpf.Behaviors
{
    /// <summary>
    /// Represents the information about a visual state group to be used
    /// by VsmChangeBehavior
    /// </summary>
    internal class ViewModelStateGroupInfo
    {
        /// <summary>
        /// The property that is watched for value change
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Method to call when the proerty's value changes
        /// </summary>
        public MethodInfo CallbackMethodInfo { get; set; }

        /// <summary>
        /// Flags if the callback has already been wired up
        /// </summary>
        public bool CallBackWiredUp { get; set; }

        /// <summary>
        /// The visual state group the property represents
        /// </summary>
        public VisualStateGroup VisualStateGroup { get; set; }
    }
}