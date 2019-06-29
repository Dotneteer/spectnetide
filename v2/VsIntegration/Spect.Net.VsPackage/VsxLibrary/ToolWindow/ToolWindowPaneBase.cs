using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.VsxLibrary.ToolWindow
{
    /// <summary>
    /// This class represents the base class of a VSX window pane
    /// </summary>
    /// <typeparam name="TControl">Control that represents the content</typeparam>
    public abstract class ToolWindowPaneBase<TControl> : ToolWindowPane,
        IVsWindowFrameEvents
        where TControl : ContentControl, new()
    {
        private uint _windowFrameEventCookie;

        /// <summary>
        /// The package that registers this tool window
        /// </summary>
        public new SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// The tool window control that displays the UI for this pane
        /// </summary>
        public new TControl Content { get; }

        /// <summary>
        /// Gets or sets the value of the initial caption
        /// </summary>
        protected string BaseCaption { get; set; }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        protected ToolWindowPaneBase() : base(null)
        {
            // --- Set content information
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
                ToolBar = new CommandID(new Guid(SpectNetPackage.COMMAND_SET_GUID), toolBarAttr.CommandId);
                ToolBarLocation = (int)toolBarAttr.Location;
            }
        }

        /// <summary>
        /// Closes the pane with the specified option
        /// </summary>
        /// <param name="options">Close option</param>
        public void ClosePane(uint options = (uint)__FRAMECLOSE.FRAMECLOSE_NoSave)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var frame = Frame as IVsWindowFrame;
            frame?.CloseFrame(options);
        }

        /// <summary>
        /// Prepares window frame events
        /// </summary>
        protected override void OnCreate()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            base.OnCreate();
            var uiShell = (IVsUIShell7)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
            _windowFrameEventCookie = uiShell.AdviseWindowFrameEvents(this);
        }

        protected override void OnClose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
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
    /// <typeparam name="TControl">Control that represents the content</typeparam>
    /// <typeparam name="TVm">The view model that binds to the control</typeparam>
    /// <remarks>
    /// This class automatically instantiates a view model instance
    /// </remarks>
    public abstract class ToolWindowPaneBase<TControl, TVm> :
        ToolWindowPaneBase<TControl>
        where TControl : ContentControl, ISupportsMvvm<TVm>, new()
        where TVm : ViewModelBase, new()
    {
        /// <summary>
        /// The view model behind this tool window
        /// </summary>
        public TVm Vm { get; }

        protected ToolWindowPaneBase()
        {
            Vm = new TVm();
            Content.SetVm(Vm);
        }
    }

}