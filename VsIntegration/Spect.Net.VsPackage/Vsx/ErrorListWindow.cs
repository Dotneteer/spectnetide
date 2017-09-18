using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class provides access to the Error List tool window within Visual Studio
    /// </summary>
    public class ErrorListWindow
    {
        private readonly ErrorListProvider _errorListProvider;

        public ErrorListWindow()
        {
            _errorListProvider = new ErrorListProvider(VsxPackage.GetPackage<SpectNetPackage>());
        }

        /// <summary>
        /// Clears the content of the Error List
        /// </summary>
        public void Clear()
        {
            _errorListProvider.Tasks.Clear();
        }

        /// <summary>
        /// Adds the specified error to the provider
        /// </summary>
        /// <param name="error"></param>
        public void AddErrorTask(ErrorTask error)
        {
            _errorListProvider.Tasks.Add(error);
        }
    }
}