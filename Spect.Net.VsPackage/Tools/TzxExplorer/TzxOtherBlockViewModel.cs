namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// This class represent the view model for those TZX blocks that has not been
    /// implemented in TZX Explorer
    /// </summary>
    public class TzxOtherBlockViewModel : TzxBlockViewModelBase
    {
        public TzxOtherBlockViewModel()
        {
            BlockType = "Not implemented";
        }
    }
}