using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class represents the base class of a VSX window pane
    /// </summary>
    public abstract class VsxToolWindowPane<TPackage, TControl>: ToolWindowPane
        where TPackage: VsxPackage
        where TControl: ContentControl, new()
    {
        /// <summary>
        /// The package that registers this tool window
        /// </summary>
        public new TPackage Package { get; }

        /// <summary>
        /// The tool window control that displays the UI for this pane
        /// </summary>
        public new TControl Content { get; }

        /// <summary>
        /// Gets the object to access Visual Studio services
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        protected VsxToolWindowPane() : base(null)
        {
            // --- Set package and content information
            ServiceProvider = Package = VsxPackage.GetPackage<TPackage>();
            base.Content = Content = new TControl();

            // --- Obtain caption info
            var typeInfo = GetType().GetTypeInfo();
            var captionAttr = typeInfo.GetCustomAttribute<CaptionAttribute>();
            if (captionAttr != null)
            {
                Caption = captionAttr.Value;
            }

            // --- Obtain toolbar info
            var toolBarAttr = typeInfo.GetCustomAttribute<ToolWindowToolbarAttribute>();
            if (toolBarAttr != null)
            {
                VsxPackage.CommandSets.TryGetValue(toolBarAttr.CommandSet, out IVsxCommandSet commandSet);
                if (commandSet != null)
                {
                    ToolBar = new CommandID(commandSet.Guid, toolBarAttr.CommandId);
                    ToolBarLocation = (int)toolBarAttr.Location;
                }
            }
        }
    }
}