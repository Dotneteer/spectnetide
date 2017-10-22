using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OutputWindow = Spect.Net.VsPackage.Vsx.Output.OutputWindow;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class is intended to be the base class of Visual Studio packages
    /// </summary>
    public abstract class VsxPackage: Package
    {
        private DTE2 _applicationObject;
        private static readonly List<Assembly> s_AssembliesToScan = new List<Assembly>();
        private static readonly Dictionary<Type, VsxPackage> s_PackageInstances =
            new Dictionary<Type, VsxPackage>();
        private static readonly Dictionary<Type, IVsxCommandSet> s_CommandSets = 
            new Dictionary<Type, IVsxCommandSet>();
        private static readonly Dictionary<Type, IVsxCommand> s_Commands =
            new Dictionary<Type, IVsxCommand>();
        private static readonly Dictionary<Type, Dictionary<int, object>> s_ToolWindowInstances =
            new Dictionary<Type, Dictionary<int, object>>();

        public static IReadOnlyDictionary<Type, VsxPackage> PackageInstances
            => new ReadOnlyDictionary<Type, VsxPackage>(s_PackageInstances);

        /// <summary>
        /// Gets the list of assemblies to scan for VsxPackage metadata
        /// </summary>
        public static IReadOnlyList<Assembly> AssembliesToScan
            => new ReadOnlyCollection<Assembly>(s_AssembliesToScan);

        /// <summary>
        /// Retrieves the command sets of this package
        /// </summary>
        public static IReadOnlyDictionary<Type, IVsxCommandSet> CommandSets 
            => new ReadOnlyDictionary<Type, IVsxCommandSet>(s_CommandSets);

        /// <summary>
        /// Gets the list of commands defined in this package
        /// </summary>
        public static IReadOnlyDictionary<Type, IVsxCommand> Commands 
            => new ReadOnlyDictionary<Type, IVsxCommand>(s_Commands);

        /// <summary>
        /// Creates a new instance of the package
        /// </summary>
        protected VsxPackage()
        {
            s_PackageInstances.Add(GetType(), this);
        }

        /// <summary>
        /// Represents the application object through which VS automation
        /// can be accessed.
        /// </summary>
        public DTE2 ApplicationObject
        {
            get
            {
                if (_applicationObject == null)
                {
                    // Get an instance of the currently running Visual Studio IDE
                    var dte = (DTE)GetService(typeof(DTE));
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    _applicationObject = dte as DTE2;
                }
                return _applicationObject;
            }
        }

        protected sealed override void Initialize()
        {
            base.Initialize();
            s_AssembliesToScan.Add(Assembly.GetExecutingAssembly());

            // --- Discover the assemblies to scan for metadata
            var typeInfo = GetType().GetTypeInfo();
            var clues = typeInfo.GetCustomAttributes<VsxClueTypeAttribute>();
            foreach (var clue in clues)
            {
                var clueAsm = clue.Value?.Assembly;
                if (clueAsm != null && !s_AssembliesToScan.Contains(clueAsm))
                {
                    s_AssembliesToScan.Add(clueAsm);
                }
            }

            // --- Discover command sets within this assembly
            ScanTypes(
                type => !type.IsAbstract && DerivesFromGeneric(type, typeof(VsxCommandSet<>)),
                type =>
                {
                    var typeInstance = (IVsxCommandSet)Activator.CreateInstance(type);
                    typeInstance.Site(this);
                    s_CommandSets.Add(type, typeInstance);
                });

            // --- Discover commands within this assembly
            ScanTypes(
                type => !type.IsAbstract && DerivesFromGeneric(type, typeof(VsxCommand<,>)),
                type =>
                {
                    var commandInstance = (IVsxCommand)Activator.CreateInstance(type);
                    if (s_CommandSets.TryGetValue(commandInstance.CommandSetType, 
                        out IVsxCommandSet commandSetInstance))
                    {
                        commandInstance.Site(commandSetInstance);
                        s_Commands.Add(type, commandInstance);
                    }
                });

            // --- Set the console output
            Console.SetOut(OutputWindow.General);

            // --- No it is time to allow the package-specific initialization
            OnInitialize();
        }

        /// <summary>
        /// Override this method to initialize the package.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Gets the specified tool window with the given instance id.
        /// </summary>
        /// <typeparam name="TWindow">Type of the tool window</typeparam>
        /// <param name="instanceId">Tool window insatnce ID</param>
        /// <returns>Tool window instance</returns>
        public TWindow GetToolWindow<TWindow>(int instanceId = 0)
            where TWindow: ToolWindowPane
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
                    new Dictionary<int, object>() {{instanceId, window}};
            }
            return window;
        }

        /// <summary>
        /// Displays the specified tool window with the given instance id.
        /// </summary>
        /// <param name="toolWindowType">Type of the tool window</param>
        /// <param name="instanceId">Tool window insatnce ID</param>
        /// <returns>Tool window instance</returns>
        public void ShowToolWindow(Type toolWindowType, int instanceId = 0)
        {
            if (toolWindowType == null)
            {
                throw new ArgumentNullException(nameof(toolWindowType));
            }
            var window = GetToolWindow(toolWindowType, instanceId);
            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        /// <summary>
        /// Traverses through all types within the assemblies involed in this
        /// VsxPackage instance and executes the specified *action* on which
        /// satisfy the *condition*. 
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="action">Action to carry out</param>
        protected void ScanTypes(Func<Type, bool> condition, Action<Type> action)
        {
            foreach (var asm in s_AssembliesToScan)
            {
                foreach (var type in asm.GetTypes())
                {
                    if (condition(type))
                    {
                        action(type);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the specified type derives diectly or indirectly
        /// from the given generic type
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <param name="ancestor">Ancestor generic type</param>
        private static bool DerivesFromGeneric(Type type, Type ancestor)
        {
            var currentType = type.BaseType;
            while (currentType != null)
            {
                if (currentType.IsGenericType
                    && currentType.GetGenericTypeDefinition() == ancestor)
                {
                    return true;
                }
                currentType = currentType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Gets the package with the specified type
        /// </summary>
        /// <typeparam name="TPackage">Type of the package</typeparam>
        /// <returns>Package instance</returns>
        public static TPackage GetPackage<TPackage>()
            where TPackage: VsxPackage
            => (TPackage)s_PackageInstances[typeof(TPackage)];
    }
}