using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class proovides helper functions that display common dialogs
    /// </summary>
    public static class VsxDialogs
    {
        /// <summary>
        /// Displays the Open File dialog and allows the user to select a file.
        /// </summary>
        /// <param name="filter">Mandatory filter such as "C# Files (*.cs)|*.cs"</param>
        /// <param name="initialPath">Optional initial file path</param>
        /// <param name="title">Optional dialog title</param>
        /// <returns></returns>
        public static string FileOpen(string filter, string initialPath = null, string title = null)
        {
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(IVsUIShell));
            if (uiShell != null)
            {
                uiShell.GetDialogOwnerHwnd(out IntPtr owner);
                var fs = new VSOPENFILENAMEW[1];
                fs[0].lStructSize = (uint) Marshal.SizeOf(typeof(VSOPENFILENAMEW));
                fs[0].pwzFilter = filter.Replace('|', '\0') + "\0";
                fs[0].hwndOwner = owner;
                fs[0].pwzDlgTitle = title;
                fs[0].nMaxFileName = 260;
                var pFileName = Marshal.AllocCoTaskMem(520);
                fs[0].pwzFileName = pFileName;
                fs[0].pwzInitialDir = initialPath;
                var nameArray = "\0".ToCharArray();
                Marshal.Copy(nameArray, 0, pFileName, nameArray.Length);
                try
                {
                    var hr = uiShell.GetOpenFileNameViaDlg(fs);
                    if (hr == VSConstants.OLE_E_PROMPTSAVECANCELLED)
                    {
                        return null;
                    }
                    ErrorHandler.ThrowOnFailure(hr);
                    return Marshal.PtrToStringAuto(fs[0].pwzFileName);
                }
                finally
                {
                    if (pFileName != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pFileName);
                    }
                }
            }
            return null;
        }
    }
}