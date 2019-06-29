using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.VsxLibrary
{
    /// <summary>
    /// This class is intended to be the base class of Visual Studio packages
    /// </summary>
    public abstract class VsxAsyncPackage: AsyncPackage
    {
        private DTE2 _applicationObject;
        private static readonly Dictionary<Type, Dictionary<int, object>> s_ToolWindowInstances =
            new Dictionary<Type, Dictionary<int, object>>();

        /// <summary>
        /// Represents the application object through which VS automation
        /// can be accessed.
        /// </summary>
        public DTE2 ApplicationObject
        {
            get
            {
                if (_applicationObject != null)
                {
                    return _applicationObject;
                }

                // --- Get an instance of the currently running Visual Studio IDE
                ThreadHelper.ThrowIfNotOnUIThread();
                var dte = (DTE)GetService(typeof(DTE));
                // ReSharper disable once SuspiciousTypeConversion.Global
                return _applicationObject = dte as DTE2;
            }
        }

        /// <summary>
        /// Gets the specified tool window with the given instance id.
        /// </summary>
        /// <typeparam name="TWindow">Type of the tool window</typeparam>
        /// <param name="instanceId">Tool window insatnce ID</param>
        /// <returns>Tool window instance</returns>
        public TWindow GetToolWindow<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane
        {
            if (s_ToolWindowInstances.TryGetValue(typeof(TWindow), out var instances))
            {
                if (instances.TryGetValue(instanceId, out var twInstance))
                {
                    return twInstance as TWindow;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the specified tool window with the given instance id.
        /// </summary>
        /// <param name="toolWindowType">Type of the tool window</param>
        /// <param name="instanceId">Tool window insatnce ID</param>
        /// <returns>Tool window instance</returns>
        public ToolWindowPane GetToolWindow(Type toolWindowType, int instanceId = 0)
        {
            var window = FindToolWindow(toolWindowType, instanceId, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            // --- Store the tool window instance reference
            if (s_ToolWindowInstances.TryGetValue(toolWindowType, out var instances))
            {
                if (!instances.ContainsKey(instanceId))
                {
                    instances.Add(instanceId, window);
                }
            }
            else
            {
                s_ToolWindowInstances[toolWindowType] =
                    new Dictionary<int, object>() { { instanceId, window } };
            }
            return window;
        }
    }
}