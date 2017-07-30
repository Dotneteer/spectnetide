namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This interface represents an object that can be sited in a
    /// VsxPackage
    /// </summary>
    public interface IVsxPackageSiteable
    {
        /// <summary>
        /// Sites the object in the specified package
        /// </summary>
        /// <param name="package">Package to site this object in</param>
        void Site(VsxPackage package);
    }
}