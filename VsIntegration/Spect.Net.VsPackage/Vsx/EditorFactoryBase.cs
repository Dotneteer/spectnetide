using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// Factory for creating o custom editor supporting the EditorPaneBase.
    /// </summary>
    public abstract class EditorFactoryBase<TEditorPane> :
        IVsEditorFactory,
        IDisposable
        where TEditorPane : new()
    {
        private ServiceProvider _serviceProvider;

        /// <summary>
        /// Gets the package owning this Editor Factory
        /// </summary>
        protected SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This parameter determines whether the method has been 
        /// called directly or indirectly by a user's code.
        /// </param>
        private void Dispose(bool disposing)
        {
            // --- If disposing equals true, dispose all managed and unmanaged resources
            if (disposing)
            {
                // --- Since we create a ServiceProvider which implements IDisposable we
                // --- also need to implement IDisposable to make sure that the ServiceProvider's
                // --- Dispose method gets called.

                if (_serviceProvider == null) return;
                _serviceProvider.Dispose();
                _serviceProvider = null;
            }
        }

        /// <summary>
        /// Used for initialization of the editor in the environment.
        /// </summary>  
        /// <param name="serviceProvider">
        /// Pointer to the service provider. Can be used to obtain instances of other 
        /// interfaces.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        public virtual int SetSite(IOleServiceProvider serviceProvider)
        {
            _serviceProvider = new ServiceProvider(serviceProvider);
            return VSConstants.S_OK;
        }

        // This method is called by the Environment (inside IVsUIShellOpenDocument::
        // OpenStandardEditor and OpenSpecificEditor) to map a LOGICAL view to a 
        // PHYSICAL view. A LOGICAL view identifies the purpose of the view that is
        // desired (e.g. a view appropriate for Debugging [LOGVIEWID_Debugging], or a 
        // view appropriate for text view manipulation as by navigating to a find
        // result [LOGVIEWID_TextView]). A PHYSICAL view identifies an actual type 
        // of view implementation that an IVsEditorFactory can create. 
        public virtual int MapLogicalView(ref Guid logicalView, out string physicalView)
        {
            physicalView = null; // --- Initialize out parameter

            // --- We support only a single physical view
            return VSConstants.LOGVIEWID_Primary == logicalView 
                ? VSConstants.S_OK 
                : VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Releases all cached interface pointers and unregisters any event sinks
        /// </summary>
        /// <returns>S_OK if the method succeeds.</returns>
        public virtual int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Used by the editor factory to create an editor instance. The environment first 
        /// determines the editor factory with the highest priority for opening the file 
        /// and then calls IVsEditorFactory.CreateEditorInstance. If the environment is 
        /// unable to instantiate the document data in that editor, it will find the editor 
        /// with the next highest priority and attempt to so that same thing. 
        /// The priority of our editor is 32 as mentioned in the attributes on the package 
        /// class.
        /// Since our editor supports opening only a single view for an instance of the 
        /// document data, if we are requested to open document data that is already 
        /// instantiated in another editor, or even our editor, we return a value 
        /// VS_E_INCOMPATIBLEDOCDATA.
        /// </summary>
        /// <param name="grfCreateDoc">
        /// Flags determining when to create the editor. Only open and silent flags 
        /// are valid.
        /// </param>
        /// <param name="pszMkDocument">Path to the file to be opened.</param>
        /// <param name="pszPhysicalView">Name of the physical view.</param>
        /// <param name="pvHier">Pointer to the IVsHierarchy interface.</param>
        /// <param name="itemid">Item identifier of this editor instance.</param>
        /// <param name="punkDocDataExisting">
        /// This parameter is used to determine if a document buffer (DocData object) 
        /// has already been created.
        /// </param>
        /// <param name="ppunkDocView">
        /// Pointer to the IUnknown interface for the DocView object.
        /// </param>
        /// <param name="ppunkDocData">
        /// Pointer to the IUnknown interface for the DocData object.
        /// </param>
        /// <param name="pbstrEditorCaption">
        /// Caption mentioned by the editor for the doc window.
        /// </param>
        /// <param name="pguidCmdUi">
        /// The Command UI Guid. Any UI element that is visible in the editor has 
        /// to use this GUID.
        /// </param>
        /// <param name="pgrfCdw">Flags for CreateDocumentWindow.</param>
        /// <returns>HRESULT result code. S_OK if the method succeeds.</returns>
        /// <remarks>
        /// Attribute usage according to FxCop rule regarding SecurityAction 
        /// requirements (LinkDemand). This method do use SecurityAction.Demand 
        /// action instead of LinkDemand because it overrides method without LinkDemand
        /// see "Demand vs. LinkDemand" article in MSDN for more details.
        /// </remarks>
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public virtual int CreateEditorInstance(
            uint grfCreateDoc,
            string pszMkDocument,
            string pszPhysicalView,
            IVsHierarchy pvHier,
            uint itemid,
            IntPtr punkDocDataExisting,
            out IntPtr ppunkDocView,
            out IntPtr ppunkDocData,
            out string pbstrEditorCaption,
            out Guid pguidCmdUi,
            out int pgrfCdw)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture,
                "Entering {0} CreateEditorInstance()", ToString()));

            // --- Initialize to null
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUi = GetType().GUID;
            pgrfCdw = 0;
            pbstrEditorCaption = null;

            // --- Validate inputs
            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }
            if (punkDocDataExisting != IntPtr.Zero)
            {
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
            }

            // --- Create the Document (editor)
            CreateViewAndData(out var newDocView, out var newDocData);
            ppunkDocView = Marshal.GetIUnknownForObject(newDocView);
            ppunkDocData = Marshal.GetIUnknownForObject(newDocData);
            pbstrEditorCaption = "";

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Override this method to create your own document view and document data.
        /// </summary>
        /// <param name="docView"></param>
        /// <param name="docData"></param>
        protected virtual void CreateViewAndData(out object docView, out object docData)
        {
            docView = new TEditorPane();
            docData = docView;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An Type that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType. -or- 
        /// A a null reference if there is no service object of type serviceType.</returns>
        protected object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}