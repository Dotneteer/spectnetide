using System;
using System.ComponentModel;

namespace Spect.Net.VsPackage.VsxLibrary.Output
{
    /// <summary>
    /// This abstract class is intended to be a base class for output window pane 
    /// definitions.
    /// </summary>
    public abstract class OutputPaneDefinition
    {
        #region Lifecycle methods

        /// <summary>
        /// Creates an instance of the class by obtaining the attributes decorating the class.
        /// </summary>
        protected OutputPaneDefinition()
        {
            InitiallyVisible = true;
            Name = "<Name not defined>";
            foreach (var attr in GetType().GetCustomAttributes(false))
            {
                switch (attr)
                {
                    case DisplayNameAttribute paneNameAttr:
                        Name = paneNameAttr.DisplayName;
                        continue;
                    case InitiallyVisibleAttribute initVisAttr:
                        InitiallyVisible = initVisAttr.Value;
                        continue;
                    case ClearWithSolutionAttribute clearWithSolAttr:
                        ClearWithSolution = clearWithSolAttr.Value;
                        continue;
                }

                if (!(attr is AutoActivateAttribute activateAttr)) continue;
                AutoActivate = activateAttr.Value;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the GUID of the output window pane.
        /// </summary>
        public virtual Guid Guid => GetType().GUID;

        /// <summary>
        /// Gets the default name of the output window pane.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the flag indicating if the output window pane is initially visible or not.
        /// </summary>
        public bool InitiallyVisible { get; }

        /// <summary>
        /// Gets the flag indicating if the output window pane is to be cleared when the
        /// current solution is closed.
        /// </summary>
        public bool ClearWithSolution { get; }

        /// <summary>
        /// Gets the flag indicating if output window pane should be automatically 
        /// activated after the first write operation.
        /// </summary>
        public bool AutoActivate { get; }

        /// <summary>
        /// Gets or internally sets the flag indicating if this output window pane is a 
        /// silent pane or not.
        /// </summary>
        /// <remarks>
        /// Silent panes do not provide output.
        /// </remarks>
        public bool IsSilent { get; internal set; }

        #endregion
    }
}