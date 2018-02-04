using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows;

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
            var uiShell = UiShell;
            if (uiShell == null) return null;

            uiShell.GetDialogOwnerHwnd(out var owner);
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

        /// <summary>
        /// Displays the Open File dialog and allows the user to select a file.
        /// </summary>
        /// <param name="filter">Mandatory filter such as "C# Files (*.cs)|*.cs"</param>
        /// <param name="initialPath">Optional initial file path</param>
        /// <param name="title">Optional dialog title</param>
        /// <returns></returns>
        public static string FileSave(string filter, string initialPath = null, string title = null)
        {
            var uiShell = UiShell;
            if (uiShell == null) return null;

            uiShell.GetDialogOwnerHwnd(out var owner);
            var saveInfo = new VSSAVEFILENAMEW[1];
            saveInfo[0].lStructSize = (uint)Marshal.SizeOf(typeof(VSSAVEFILENAMEW));
            saveInfo[0].pwzFilter = filter.Replace('|', '\0') + "\0";
            saveInfo[0].hwndOwner = owner;
            saveInfo[0].pwzDlgTitle = title;
            saveInfo[0].nMaxFileName = 260;
            var pFileName = Marshal.AllocCoTaskMem(520);
            saveInfo[0].pwzFileName = pFileName;
            saveInfo[0].pwzInitialDir = Path.GetDirectoryName(initialPath);
            var nameArray = (Path.GetFileName(initialPath) + "\0").ToCharArray();
            Marshal.Copy(nameArray, 0, pFileName, nameArray.Length);
            try
            {
                int hr = uiShell.GetSaveFileNameViaDlg(saveInfo);
                if (hr == VSConstants.OLE_E_PROMPTSAVECANCELLED)
                {
                    return null;
                }
                ErrorHandler.ThrowOnFailure(hr);
                return Marshal.PtrToStringAuto(saveInfo[0].pwzFileName);
            }
            finally
            {
                if (pFileName != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pFileName);
                }
            }
        }

        /// <summary>
        /// Displays a Visual Studio message box.
        /// </summary>
        /// <param name="text">Message text</param>
        /// <returns>
        /// Result of the MessageBox.
        /// </returns>
        public static VsxDialogResult Show(string text)
        {
            return ShowInternal(null, text, string.Empty, 0, MessageBoxButton.OK,
                0, VsxMessageBoxIcon.Information, false);
        }

        /// <summary>
        /// Displays a Visual Studio message box.
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="text">Message text</param>
        /// <returns>
        /// Result of the MessageBox.
        /// </returns>
        public static VsxDialogResult Show(string text, string title)
        {
            return ShowInternal(title, text, string.Empty, 0, MessageBoxButton.OK,
                0, VsxMessageBoxIcon.Information, false);
        }

        /// <summary>
        /// Displays a Visual Studio message box.
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="text">Message text</param>
        /// <param name="buttons">Buttons to show on the message box</param>
        /// <returns>
        /// Result of the MessageBox.
        /// </returns>
        public static VsxDialogResult Show(string text, string title, MessageBoxButton buttons)
        {
            return ShowInternal(title, text, string.Empty, 0, buttons,
                0, VsxMessageBoxIcon.Information, false);
        }

        /// <summary>
        /// Displays a Visual Studio message box.
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="text">Message text</param>
        /// <param name="buttons">Buttons to show on the message box</param>
        /// <param name="icon">Icon to display in the message box.</param>
        /// <returns>
        /// Result of the MessageBox.
        /// </returns>
        public static VsxDialogResult Show(string text, string title,
            MessageBoxButton buttons, VsxMessageBoxIcon icon)
        {
            return ShowInternal(title, text, string.Empty, 0, buttons, 0, icon, false);
        }

        /// <summary>
        /// Displays a Visual Studio message box.
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="text">Message text</param>
        /// <param name="buttons">Buttons to show on the message box</param>
        /// <param name="defaultButton">Default message box button.</param>
        /// <param name="icon">Icon to display in the message box.</param>
        /// <returns>
        /// Result of the MessageBox.
        /// </returns>
        public static VsxDialogResult Show(string text, string title,
            MessageBoxButton buttons, VsxMessageBoxIcon icon, int defaultButton)
        {
            return ShowInternal(title, text, string.Empty, 0, buttons,
                                defaultButton, icon, false);
        }

        /// <summary>
        /// Displays a Visual Studio message box.
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="message">Message text</param>
        /// <param name="helpFile">Help file name</param>
        /// <param name="helpTopic">Help topic identifier</param>
        /// <param name="buttons">Buttons to show on the message box</param>
        /// <param name="defButton">Default message box button.</param>
        /// <param name="icon">Icon to display in the message box.</param>
        /// <param name="sysAlert">MB_SYSTEMMODAL flag</param>
        /// <returns>
        /// MessageBox result converted to DialogResult.
        /// </returns>
        private static VsxDialogResult ShowInternal(string title, string message, string helpFile,
                                                 uint helpTopic, MessageBoxButton buttons, int defButton,
                                                 VsxMessageBoxIcon icon, bool sysAlert)
        {
            var clsid = Guid.Empty;
            ErrorHandler.ThrowOnFailure(UiShell.ShowMessageBox(
                                          0,
                                          ref clsid,
                                          title,
                                          message,
                                          helpFile,
                                          helpTopic,
                                          VsxConverter.ConvertToOleMsgButton(buttons),
                                          VsxConverter.ConvertToOleMsgDefButton(defButton),
                                          VsxConverter.ConvertToOleMsgIcon(icon),
                                          sysAlert ? 1 : 0,
                                          out var result));
            return VsxConverter.Win32ResultToDialogResult(result);
        }

        /// <summary>
        /// Gets the IVsUIShell service instance.
        /// </summary>
        private static IVsUIShell UiShell => 
            (IVsUIShell)Package.GetGlobalService(typeof(IVsUIShell));
    }

    /// <summary>
    /// Message box icons to show
    /// </summary>
    public enum VsxMessageBoxIcon
    {
        Asterisk,
        Error,
        Exclamation,
        Question,
        Information
    }


    /// <summary>
    /// Message box dialog results
    /// </summary>
    public enum VsxDialogResult
    {
        Ok,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }
}