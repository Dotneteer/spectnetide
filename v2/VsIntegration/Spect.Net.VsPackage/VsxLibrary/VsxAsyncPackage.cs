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

    }
}