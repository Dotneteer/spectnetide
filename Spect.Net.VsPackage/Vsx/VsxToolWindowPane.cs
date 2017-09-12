using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class represents the base class of a VSX window pane
    /// </summary>
    /// <typeparam name="TPackage">Host package type</typeparam>
    /// <typeparam name="TControl">Control that represents the content</typeparam>
    public abstract class VsxToolWindowPane<TPackage, TControl>: ToolWindowPane, 
        IVsWindowFrameEvents
        where TPackage: VsxPackage
        where TControl: ContentControl, new()
    {
        private uint _windowFrameEventCookie;

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

        /// <summary>
        /// Gets or sets the value of the initial caption
        /// </summary>
        protected string BaseCaption { get; set; }


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
                Caption = BaseCaption = captionAttr.Value;
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

        /// <summary>
        /// Closes the pane with the specified option
        /// </summary>
        /// <param name="options">Close option</param>
        public void ClosePane(uint options = (uint)__FRAMECLOSE.FRAMECLOSE_NoSave)
        {
            var frame = Frame as IVsWindowFrame;
            frame?.CloseFrame(options);
        }

        /// <summary>
        /// Prepares window frame events
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            var uiShell = (IVsUIShell7)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
            _windowFrameEventCookie = uiShell.AdviseWindowFrameEvents(this);
        }

        protected override void OnClose()
        {
            var uiShell = (IVsUIShell7)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
            uiShell.UnadviseWindowFrameEvents(_windowFrameEventCookie);
            base.OnClose();
        }

        /// <summary>Called when a new IVsWindowFrame is created.</summary>
        /// <param name="frame">The frame.</param>
        public virtual void OnFrameCreated(IVsWindowFrame frame)
        {
        }

        /// <summary>Called when an IVsWindowFrame is permanently closed.</summary>
        /// <param name="frame">The frame.</param>
        public virtual void OnFrameDestroyed(IVsWindowFrame frame)
        {
        }

        /// <summary>Called when the IsVisible property of an IVsWindowFrame changes.</summary>
        /// <param name="frame">The frame.</param>
        /// <param name="newIsVisible">The new IsVisible value.</param>
        public virtual void OnFrameIsVisibleChanged(IVsWindowFrame frame, bool newIsVisible)
        {
        }

        /// <summary>Called when the IsOnScreen property of an IVsWindowFrame changes.</summary>
        /// <param name="frame">The frame.</param>
        /// <param name="newIsOnScreen">The new IsOnScreen value.</param>
        public virtual void OnFrameIsOnScreenChanged(IVsWindowFrame frame, bool newIsOnScreen)
        {
        }

        /// <summary>Called when the active IVsWindowFrame changes.</summary>
        /// <param name="oldFrame">The old active frame.</param>
        /// <param name="newFrame">The new active frame.</param>
        public virtual void OnActiveFrameChanged(IVsWindowFrame oldFrame, IVsWindowFrame newFrame)
        {
        }
    }

    /// <summary>
    /// This class represents the base class of a VSX window pane
    /// </summary>
    /// <typeparam name="TPackage">Host package type</typeparam>
    /// <typeparam name="TControl">Control that represents the content</typeparam>
    /// <typeparam name="TVm">The view model that binds to the control</typeparam>
    /// <remarks>
    /// This class automatically instantiates a view model instance
    /// </remarks>
    public class VsxToolWindowPane<TPackage, TControl, TVm> : 
        VsxToolWindowPane<TPackage, TControl> 
        where TPackage : VsxPackage 
        where TControl : ContentControl, ISupportsMvvm<TVm>, new()
        where TVm: ViewModelBase, new()
    {
        public TVm Vm { get; }

        public VsxToolWindowPane()
        {
            Vm = new TVm();
            Content.SetVm(Vm);
        }
    }
}