using GalaSoft.MvvmLight;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This interface defines the behavior of an object
    /// that supports using an associated view model.
    /// </summary>
    /// <typeparam name="TVm">View model type</typeparam>
    public interface ISupportsMvvm<TVm>
        where TVm: ViewModelBase
    {
        /// <summary>
        /// Gets the view model instance
        /// </summary>
        TVm Vm { get; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void SetVm(TVm vm);
    }
}