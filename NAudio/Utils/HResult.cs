using System.Runtime.InteropServices;

namespace NAudio.Utils
{
    /// <summary>
    /// HResult
    /// </summary>
    public static class HResult
    {
        /// <summary>
        /// S_OK
        /// </summary>
        public const int S_OK = 0;
        /// <summary>
        /// S_FALSE
        /// </summary>
        public const int S_FALSE = 1;
        /// <summary>
        /// E_INVALIDARG (from winerror.h)
        /// </summary>
        public const int E_INVALIDARG = unchecked((int)0x80000003);
        /// <summary>
        /// MAKE_HRESULT macro
        /// </summary>
        public static int MAKE_HRESULT(int sev, int fac, int code)
        {
            return (int) (((uint)sev) << 31 | ((uint)fac) << 16 | ((uint)code));
        }

        /// <summary>
        /// Helper to deal with the fact that in Win Store apps,
        /// the HResult property name has changed
        /// </summary>
        /// <param name="exception">COM Exception</param>
        /// <returns>The HResult</returns>
        public static int GetHResult(this COMException exception)
        {
#if NETFX_CORE
            return exception.HResult;
#else
            return exception.ErrorCode;
#endif
        }
    }

}
