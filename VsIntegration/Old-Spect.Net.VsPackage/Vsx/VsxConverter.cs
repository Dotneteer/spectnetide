using System.Windows;
using Microsoft.VisualStudio.Shell.Interop;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This static class provides conversion functions between interop types and managed
    /// VSX types.
    /// </summary>
    public static class VsxConverter
    {
        /// <summary>
        /// Converts <see cref="MessageBoxButton"/> values to corresponding 
        /// <see cref="OLEMSGBUTTON"/> values.
        /// </summary>
        /// <param name="button">MessageBoxButtons value to convert</param>
        /// <returns>
        /// OLEMSGBUTTON representation of the MessageBoxButtons input.
        /// </returns>
        public static OLEMSGBUTTON ConvertToOleMsgButton(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    return OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL;
                case MessageBoxButton.YesNo:
                    return OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
                case MessageBoxButton.YesNoCancel:
                    return OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
                default:
                    return OLEMSGBUTTON.OLEMSGBUTTON_OK;
            }
        }

        /// <summary>
        /// Converts integer values to corresponding
        /// <see cref="OLEMSGDEFBUTTON"/> values.
        /// </summary>
        /// <param name="button">MessageBoxDefaultButton value to convert</param>
        /// <returns>
        /// OLEMSGDEFBUTTON representation of the MessageBoxDefaultButton input.
        /// </returns>
        public static OLEMSGDEFBUTTON ConvertToOleMsgDefButton(int button)
        {
            switch (button)
            {
                case 1:
                    return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND;
                case 2:
                    return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_THIRD;
                case 3:
                    return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FOURTH;
                default:
                    return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            }
        }

        /// <summary>
        /// Converts <see cref="VsxMessageBoxIcon"/> values to corresponding
        /// <see cref="OLEMSGICON"/> values.
        /// </summary>
        /// <param name="icon">MessageBoxIcon value to convert</param>
        /// <returns>
        /// OLEMSGICON representation of the MessageBoxIcon input.
        /// </returns>
        public static OLEMSGICON ConvertToOleMsgIcon(VsxMessageBoxIcon icon)
        {
            switch (icon)
            {
                case VsxMessageBoxIcon.Asterisk:
                case VsxMessageBoxIcon.Information:
                    return OLEMSGICON.OLEMSGICON_INFO;
                case VsxMessageBoxIcon.Error:
                    return OLEMSGICON.OLEMSGICON_CRITICAL;
                case VsxMessageBoxIcon.Exclamation:
                    return OLEMSGICON.OLEMSGICON_WARNING;
                case VsxMessageBoxIcon.Question:
                    return OLEMSGICON.OLEMSGICON_QUERY;
                default:
                    return OLEMSGICON.OLEMSGICON_NOICON;
            }
        }

        /// <summary>
        /// Converts the WIN32 dialog result values to DialogResult enumeration values.
        /// </summary>
        /// <param name="value">Integer value to convert</param>
        /// <returns>
        /// DialogResult representation.
        /// </returns>
        public static VsxDialogResult Win32ResultToDialogResult(int value)
        {
            switch (value)
            {
                case 1:
                    return VsxDialogResult.Ok;
                case 2:
                    return VsxDialogResult.Cancel;
                case 3:
                    return VsxDialogResult.Abort;
                case 4:
                    return VsxDialogResult.Retry;
                case 5:
                    return VsxDialogResult.Ignore;
                case 6:
                    return VsxDialogResult.Yes;
                default:
                    return VsxDialogResult.No;
            }
        }
    }
}
