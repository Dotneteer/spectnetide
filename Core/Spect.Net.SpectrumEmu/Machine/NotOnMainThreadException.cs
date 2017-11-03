using System;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// The virtual machine's controller raises this exception if it does not
    /// run on the main thread whan a control method is invoked.
    /// </summary>
    public class NotOnMainThreadException: InvalidOperationException
    {
        public NotOnMainThreadException():
            base ("The virtual machine's controller methods should be called from the main thread.")
        {
        }
    }
}