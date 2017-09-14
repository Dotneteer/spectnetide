using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class represents a command set that belongs to a package
    /// </summary>
    /// <typeparam name="TPackage">Package that holds this command set</typeparam>
    public abstract class VsxCommandSet<TPackage>: IVsxCommandSet
        where TPackage: VsxPackage
    {
        /// <summary>
        /// The package that owns this command set
        /// </summary>
        public TPackage Package { get; private set; }

        /// <summary>
        /// The command set identifier (Guid)
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// The command set identifier string
        /// </summary>
        public string GuidString { get; private set; }

        /// <summary>
        /// Sites the object in the specified package
        /// </summary>
        /// <param name="package"></param>
        public void Site(VsxPackage package)
        {
            Package = (TPackage)package;
            var guid = GetType().GetTypeInfo().GetCustomAttribute<GuidAttribute>();
            if (guid == null) return;

            GuidString = guid.Value;
            Guid = new Guid(guid.Value);
        }
    }
}