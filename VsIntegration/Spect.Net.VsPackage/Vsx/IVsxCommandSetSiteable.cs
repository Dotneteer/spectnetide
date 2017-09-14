namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This interface represents an object that can be sited in a
    /// VsxCommandSet
    /// </summary>
    public interface IVsxCommandSetSiteable
    {
        /// <summary>
        /// Sites the object in the specified command set
        /// </summary>
        /// <param name="commandSet">Command set to site this object in</param>
        void Site(IVsxCommandSet commandSet);
    }
}