using System.ComponentModel.Composition;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// This interface provides the host package for a MEF component
    /// </summary>
    public interface IHostPackageProvider
    {
        SpectNetPackage Package { get; }
    }

    /// <summary>
    /// Implements the host package provider
    /// </summary>
    [Export(typeof(IHostPackageProvider))]
    internal class HostPackageProvider : IHostPackageProvider
    {
        public SpectNetPackage Package => VsxPackage.GetPackage<SpectNetPackage>();
    }
}