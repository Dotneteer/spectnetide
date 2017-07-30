using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class is intended to be the base class of Visual Studio packages
    /// </summary>
    public abstract class VsxPackage: Package
    {
        private DTE2 _applicationObject;
        private readonly Dictionary<Type, IVsxCommandSet> _commandSets = 
            new Dictionary<Type, IVsxCommandSet>();
        private readonly List<Assembly> _assembliesToScan = new List<Assembly>();
        private readonly Dictionary<Type, IVsxCommand> _commands =
            new Dictionary<Type, IVsxCommand>();

        /// <summary>
        /// Gets the list of assemblies to scan for VsxPackage metadata
        /// </summary>
        public IReadOnlyList<Assembly> AssembliesToScan
            => new ReadOnlyCollection<Assembly>(_assembliesToScan);

        /// <summary>
        /// Retrieves the command sets of this package
        /// </summary>
        public IReadOnlyDictionary<Type, IVsxCommandSet> CommandSets 
            => new ReadOnlyDictionary<Type, IVsxCommandSet>(_commandSets);

        /// <summary>
        /// Gets the list of commands defined in this package
        /// </summary>
        public IReadOnlyDictionary<Type, IVsxCommand> Commands 
            => new ReadOnlyDictionary<Type, IVsxCommand>(_commands);

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
            _assembliesToScan.Add(Assembly.GetExecutingAssembly());

            // --- Discover the assemblies to scan for metadata
            var typeInfo = GetType().GetTypeInfo();
            var clues = typeInfo.GetCustomAttributes<VsxClueTypeAttribute>();
            foreach (var clue in clues)
            {
                var clueAsm = clue.Value?.Assembly;
                if (clueAsm != null && !_assembliesToScan.Contains(clueAsm))
                {
                    _assembliesToScan.Add(clueAsm);
                }
            }

            // --- Discover command sets within this assembly
            ScanTypes(
                type => !type.IsAbstract && DerivesFromGeneric(type, typeof(VsxCommandSet<>)),
                type =>
                {
                    var typeInstance = (IVsxCommandSet)Activator.CreateInstance(type);
                    typeInstance.Site(this);
                    _commandSets.Add(type, typeInstance);
                });

            // --- Discover commands within this assembly
            ScanTypes(
                type => !type.IsAbstract && DerivesFromGeneric(type, typeof(VsxCommand<,>)),
                type =>
                {
                    var commandInstance = (IVsxCommand)Activator.CreateInstance(type);
                    if (_commandSets.TryGetValue(commandInstance.CommandSetType, 
                        out IVsxCommandSet commandSetInstance))
                    {
                        commandInstance.Site(commandSetInstance);
                        _commands.Add(type, commandInstance);
                    }
                });

            // --- No it is time to allow the package-specific initialization
            OnInitialize();
        }

        /// <summary>
        /// Override this method to initialize the package.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Traverses through all types within the assemblies involed in this
        /// VsxPackage instance and executes the specified *action* on which
        /// satisfy the *condition*. 
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="action">Action to carry out</param>
        protected void ScanTypes(Func<Type, bool> condition, Action<Type> action)
        {
            foreach (var asm in _assembliesToScan)
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
        private bool DerivesFromGeneric(Type type, Type ancestor)
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
    }
}