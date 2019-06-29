using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
// ReSharper disable UnusedMember.Global

namespace Spect.Net.VsPackage.VsxLibrary
{
    /// <summary>
    /// This static class represents behavior of the DTE2 object belonging to the current VS IDE 
    /// instance.
    /// </summary>
    public static class VsIde
    {
        #region Private fields

        private static DTE2 s_DteInstance;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the DTE2 object representing the VS IDE instance.
        /// </summary>
        public static DTE2 DteInstance =>
            s_DteInstance
                ?? (s_DteInstance = Package.GetGlobalService(typeof(DTE)) as DTE2);

        /// <summary>
        /// Gets a ToolWindows object used as a shortcut for finding tool windows.
        /// </summary>
        public static ToolWindows ToolWindows =>
            DteInstance.ToolWindows;

        #endregion

        #region Public methods

        /// <summary>
        /// Gets an interface or object that is late-bound to the DTE object and can be accessed by 
        /// name at run time.
        /// </summary>
        /// <param name="objectType">The name of the object to retrieve.</param>
        /// <returns>
        /// An interface or object that is late-bound to the DTE object.
        /// </returns>
        /// <remarks>
        /// GetObject is most useful in languages that do not support early binding. In this case, you 
        /// can name the specific interface or object you want, for example, DTE.GetObject("VCProjects").
        /// </remarks>
        public static object GetObject(string objectType)
        {
            return DteInstance.GetObject(objectType);
        }

        /// <summary>
        /// Gets an interface or object that is late-bound to the DTE object and can be accessed by 
        /// name at run time.
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve.</typeparam>
        /// <param name="objectType">The name of the object to retrieve.</param>
        /// <returns>
        /// An interface or object that is late-bound to the DTE object. Null, if the object does not 
        /// exists or cannot be converted to the specified type.
        /// </returns>
        /// <remarks>
        /// GetObject is most useful in languages that do not support early binding. In this case, you 
        /// can name the specific interface or object you want, for example, DTE.GetObject("VCProjects").
        /// </remarks>
        public static T GetObject<T>(string objectType)
          where T : class
        {
            return GetObject(objectType) as T;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="commandName">Required. The name of the command to invoke.</param>
        /// <param name="commandArgs">
        /// Optional. A string containing the same arguments you would supply if you were invoking 
        /// the command from the Command window. If a string is supplied, it is passed to the command 
        /// line as the command's first argument and is parsed to form the various arguments for the 
        /// command. This is similar to how commands are invoked in the Command window.
        /// </param>
        /// <remarks>
        /// ExecuteCommand runs commands or macros listed in the Keyboard section of the Environment 
        /// panel of the Options dialog box on the Tools menu.
        /// You can also invoke commands or macros by running them from the command line, in the 
        /// Command window, or by pressing toolbar buttons or keystrokes associated with them.
        /// ExecuteCommand cannot execute commands that are currently disabled in the environment. 
        /// The Build method, for example, will not execute while a build is currently in progress.
        /// ExecuteCommand implicitly pauses macro recording so that the executing command does not 
        /// emit macro code. This prevents double code emission when recording and invoking macros as 
        /// part of what you are recording.
        /// </remarks>
        public static void ExecuteCommand(string commandName, string commandArgs)
        {
            DteInstance.ExecuteCommand(commandName, commandArgs);
        }

        /// <summary>
        /// Restarts Visual Studio.
        /// </summary>
        public static void RestartVs()
        {
            var vs = new System.Diagnostics.Process();
            var args = Environment.GetCommandLineArgs();

            vs.StartInfo.FileName = Path.GetFullPath(args[0]);
            vs.StartInfo.Arguments = string.Join(" ", args, 1, args.Length - 1);
            vs.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            vs.Start();

            DteInstance.Quit();
        }

        #endregion
    }
}