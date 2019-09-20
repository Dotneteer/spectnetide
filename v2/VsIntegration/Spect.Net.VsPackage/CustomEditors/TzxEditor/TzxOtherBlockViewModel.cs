namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This class represent the view model for those TZX blocks that has not been
    /// implemented in TZX Explorer
    /// </summary>
    public class TzxOtherBlockViewModel : TapeBlockViewModelBase
    {
        public TzxOtherBlockViewModel()
        {
            BlockType = "Not implemented";
        }
    }
}
