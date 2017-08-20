using Spect.Net.VsPackage.CustomEditors.TzxEditor;

namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// This class represents the view model of the TZX Explorer tool window
    /// </summary>
    public class TzxExplorerViewModel: TzxViewModel
    {
        private string _fileName;
        private string _latestPath;
        private bool _loaded;

        /// <summary>
        /// The full name of the selected TZX file
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => Set(ref _fileName, value);
        }

        /// <summary>
        /// The last path used to select a TZX file
        /// </summary>
        public string LatestPath
        {
            get => _latestPath;
            set => Set(ref _latestPath, value);
        }

        /// <summary>
        /// Indicates if a file is loaded
        /// </summary>
        public bool Loaded
        {
            get => _loaded;
            set => Set(ref _loaded, value);
        }

        public TzxExplorerViewModel()
        {
            FileName = null;
            LatestPath = null;
            Loaded = false;
        }
    }
}