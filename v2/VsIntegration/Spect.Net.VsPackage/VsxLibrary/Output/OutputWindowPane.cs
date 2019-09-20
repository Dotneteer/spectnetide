using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
// ReSharper disable UnusedMember.Global

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.VsxLibrary.Output
{
    /// <summary>
    /// This class is a wrapper class around an IVsOutputWindowPane instance to manage output handling 
    /// for the window pane.
    /// </summary>
    /// <inheritdoc />
    public sealed class OutputWindowPane : TextWriter
    {
        #region Private fields

        private readonly OutputPaneDefinition _paneDefinition;
        private readonly IVsOutputWindowPane _pane;
        private readonly string _name;
        private bool _hasOutput;

        #endregion

        #region Lifecycle methods

        /// <summary>
        /// Creates an output pane instance using the specified output pane definition and 
        /// IVsOutputWindowPane instance.
        /// </summary>
        /// <param name="paneDef">Pane definition instance</param>
        /// <param name="pane">Physical output window pane</param>
        /// <remarks>
        /// This constructor is to be used only by the OutputWindow class.
        /// </remarks>
        internal OutputWindowPane(OutputPaneDefinition paneDef, IVsOutputWindowPane pane)
        {
            _paneDefinition = paneDef;
            if (paneDef != null) _name = paneDef.Name;
            _pane = pane;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the name of the output window pane.
        /// </summary>
        public string Name
        {
            get
            {
                if (IsVirtual) return _name;
                var name = string.Empty;
                _pane.GetName(ref name);
                return name;
            }
        }

        /// <summary>
        /// Checks if this output pane is virtual or not.
        /// </summary>
        public bool IsVirtual => 
            _paneDefinition != null && _paneDefinition.IsSilent 
                || _pane == null;

        #endregion

        #region Public methods

        /// <summary>
        /// Output a message with the associated file name and position of the file
        /// we can use the navigation buttons or double click the line and Visual Studio 
        /// will open the file and position the caret in the corresponding line an column.
        /// </summary>
        public void Write(string path, int line, int column, string message)
        {
            var mask = $"{path}({line},{column}) : {message}{Environment.NewLine}";
            OutputString(mask);
        }

        /// <summary>
        /// Activates this output window pane, shows the pane in the output window.
        /// </summary>
        public void Activate()
        {
            if (IsVirtual) return;
            _pane.Activate();
        }

        /// <summary>
        /// Hides this output window pane, closes it in the output window.
        /// </summary>
        public void Hide()
        {
            if (IsVirtual) return;
            _pane.Hide();
        }

        /// <summary>
        /// Clears the content of the output window pane.
        /// </summary>
        public void Clear()
        {
            if (IsVirtual) return;
            _pane.Clear();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Outputs the specified string to this window pane.
        /// </summary>
        /// <param name="output">String to send to the output.</param>
        private void OutputString(string output)
        {
            if (IsVirtual) return;
            //_pane.OutputStringThreadSafe(output);
            _pane.OutputString(output);
            if (_paneDefinition.AutoActivate && !_hasOutput)
            {
                Activate();
            }
            _hasOutput = true;
        }

        #endregion

        #region TextWriter overrides

        /// <inheritdoc />
        public override Encoding Encoding => Encoding.UTF8;

        /// <inheritdoc />
        public override void Write(char value)
        {
            Write(value.ToString());
        }

        /// <inheritdoc />
        public override void Write(char[] buffer, int index, int count)
        {
            var sb = new StringBuilder(count + 2);
            for (var i = 0; i < count; i++)
            {
                sb.Append(buffer[index + i]);
            }
            Write(sb.ToString());
        }

        /// <inheritdoc />
        public override void Write(string output)
        {
            OutputString(output);
        }

        #endregion
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
