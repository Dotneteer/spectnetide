using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class represents the base class of a VSX window pane
    /// </summary>
    public abstract class VsxToolWindowPane<TPackage, TControl>: ToolWindowPane, IVsWindowFrameNotify3
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

        /// <summary>
        /// Prepares window frame events
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            InvokeFrameMethod(f => f.SetProperty((int)__VSFPROPID.VSFPROPID_ViewHelper, this));
        }

        /// <summary>Notifies the VSPackage of a change in the window's display state.</summary>
        /// <param name="fShow">[in] Specifies the reason for the display state change. Value taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW" /> enumeration.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        public virtual int OnShow(int fShow) => VSConstants.S_OK;

        /// <summary>Notifies the VSPackage that a window is being moved.</summary>
        /// <param name="x">[in] New horizontal position.</param>
        /// <param name="y">[in] New vertical position.</param>
        /// <param name="w">[in] New window width.</param>
        /// <param name="h">[in] New window height.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        public virtual int OnMove(int x, int y, int w, int h) => VSConstants.S_OK;

        /// <summary>Notifies the VSPackage that a window is being resized.</summary>
        /// <param name="x">[in] New horizontal position.</param>
        /// <param name="y">[in] New vertical position.</param>
        /// <param name="w">[in] New window width.</param>
        /// <param name="h">[in] New window height.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        public int OnSize(int x, int y, int w, int h) => VSConstants.S_OK;

        /// <summary>Notifies the VSPackage that a window's docked state is being altered.</summary>
        /// <param name="fDockable">[in] <see langword="true" /> if the window frame is being docked.</param>
        /// <param name="x">[in] Horizontal position of undocked window.</param>
        /// <param name="y">[in] Vertical position of undocked window.</param>
        /// <param name="w">[in] Width of undocked window.</param>
        /// <param name="h">[in] Height of undocked window.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        public int OnDockableChange(int fDockable, int x, int y, int w, int h) => VSConstants.S_OK;

        /// <summary>Notifies the VSPackage that a window frame is closing and tells the environment what action to take.</summary>
        /// <param name="pgrfSaveOptions">[in, out] Specifies options for saving window content. Values are taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__FRAMECLOSE" /> enumeration.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        public int OnClose(ref uint pgrfSaveOptions) => VSConstants.S_OK;

        private void InvokeFrameMethod(Action<IVsWindowFrame> action)
        {
            var windowFrame = Frame as IVsWindowFrame;
            if (windowFrame != null) action(windowFrame);
        }
    }
}