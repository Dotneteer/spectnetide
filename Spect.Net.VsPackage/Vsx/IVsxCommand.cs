using System;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This interface represents a VSX command
    /// </summary>
    public interface IVsxCommand : IVsxCommandSetSiteable
    {
        /// <summary>
        /// The id of the command
        /// </summary>
        int CommandId { get; }

        /// <summary>
        /// Obtains the OleMenuCommand behind this VsxCommand
        /// </summary>
        OleMenuCommand OleMenuCommand { get; }

        /// <summary>
        /// Gets the type of command set
        /// </summary>
        Type CommandSetType { get; }

    }
}