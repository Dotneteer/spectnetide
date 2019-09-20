using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace Spect.Net.VsPackage.VsxLibrary.Editor
{
    /// <summary>
    /// This class is intended to be the base class of editor panes
    /// </summary>
    /// <typeparam name="TControl">Type of the control that represents the editor</typeparam>
    /// <typeparam name="TFactory">Type of the editor factory</typeparam>
    [ComVisible(true)]
    public abstract class EditorPaneBase<TFactory, TControl> : WindowPane,
        IOleComponent,
        IVsDeferredDocView,
        IVsLinkedUndoClient,
        IVsPersistDocData,
        IPersistFileFormat

        where TFactory : IVsEditorFactory
        where TControl : Control, new()
    {
        private IOleUndoManager _undoManager;
        private uint _componentId;
        private bool _isDirty;
        private bool _loading;
        private bool _noScribbleMode;
        private bool _gettingCheckoutStatus;

        // --- Our editor will support only one file format, this is its index.
        private const uint FILE_FORMAT_INDEX = 0;
        private const char END_LINE_CHAR = (char)10;

        /// <summary>
        /// The package hosting this editor pane
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Gets the name of the file currently loaded
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The control instance that hosts the editor
        /// </summary>
        public TControl EditorControl { get; private set; }

        /// <summary>
        /// Property to access VsUiShell
        /// </summary>
        private IVsUIShell VsUiShell { get; }

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public abstract string FileExtensionUsed { get; }

        public virtual Guid FactoryGuid => typeof(TFactory).GUID;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected EditorPaneBase()
        {
            VsUiShell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
        }

        protected override void Initialize()
        {
            base.Initialize();

            // --- Create and initialize the editor
            var componentManager = (IOleComponentManager)GetService(typeof(SOleComponentManager));
            if (_componentId == 0 && componentManager != null)
            {
                var crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime | (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal | (uint)_OLECADVF.olecadvfRedrawOff | (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 100;
                var hr = componentManager.FRegisterComponent(this, crinfo, out _componentId);
                ErrorHandler.Succeeded(hr);
            }

            _undoManager = (IOleUndoManager)GetService(typeof(SOleUndoManager));
            var linkCapableUndoMgr = (IVsLinkCapableUndoManager)_undoManager;
            linkCapableUndoMgr?.AdviseLinkedUndoClient(this);

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            Content = EditorControl = new TControl();
            OnEditorControlInitialized();

            if (!(GetService(typeof(IMenuCommandService)) is IMenuCommandService mcs)) return;

            // Now create one object derived from MenuCommnad for each command defined in
            // the CTC file and add it to the command service.

            // For each command we have to define its id that is a unique Guid/integer pair, then
            // create the OleMenuCommand object for this command. The EventHandler object is the
            // function that will be called when the user will select the command. Then we add the 
            // OleMenuCommand to the menu service.  The addCommand helper function does all this for us.
            AddCommand(mcs, VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.NewWindow,
                OnNewWindow, OnQueryNewWindow);
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected virtual void OnEditorControlInitialized()
        {
        }

        /// <summary>
        /// The shell invokes this method when closing the editor pane
        /// </summary>
        protected override void OnClose()
        {
            // --- Unhook from Undo related services
            if (_undoManager != null)
            {
                var linkCapableUndoMgr = (IVsLinkCapableUndoManager)_undoManager;
                linkCapableUndoMgr?.UnadviseLinkedUndoClient();
                var lco = (IVsLifetimeControlledObject)_undoManager;
                lco.SeverReferencesToOwner();
                _undoManager = null;
            }

            var mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
            mgr?.FRevokeComponent(_componentId);

            Dispose(true);
            base.OnClose();
        }

        /// <summary>Reserved.</summary>
        /// <param name="dwReserved">Reserved.</param>
        /// <param name="message">Reserved.</param>
        /// <param name="wParam">Reserved.</param>
        /// <param name="lParam">Reserved.</param>
        /// <returns>Always returns <see langword="true" />.</returns>
        int IOleComponent.FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return VSConstants.S_OK;
        }

        /// <summary>Processes the message before it is translated and dispatched.</summary>
        /// <param name="pMsg">The message.</param>
        /// <returns>
        /// <see langword="true" /> if the message is consumed, <see langword="false" /> otherwise.</returns>
        int IOleComponent.FPreTranslateMessage(MSG[] pMsg)
        {
            return VSConstants.S_OK;
        }

        /// <summary>Notifies the component when the application enters or exits the specified state.</summary>
        /// <param name="uStateId">The state, from <see cref="T:Microsoft.VisualStudio.OLE.Interop._OLECSTATE" />. </param>
        /// <param name="fEnter">
        /// <see langword="true" /> if the application is entering the state, <see langword="false" /> if it is exiting the state.</param>
        void IOleComponent.OnEnterState(uint uStateId, int fEnter)
        {
        }

        /// <summary>Notifies the component when the host application gains or loses activation.</summary>
        /// <param name="fActive">True if the application is being activated, false if it is losing activation.</param>
        /// <param name="dwOtherThreadId">The ID of the thread that owns the window.</param>
        void IOleComponent.OnAppActivate(int fActive, uint dwOtherThreadId)
        {
        }

        /// <summary>Notifies the active component that it has lost its active status because the host or another component has become active.</summary>
        void IOleComponent.OnLoseActivation()
        {
        }

        /// <summary>Notifies the component when a new object is being activated.</summary>
        /// <param name="pic">The component that is being activated</param>
        /// <param name="fSameComponent">
        /// <see langword="true" /> if <paramref name="pic" /> is the same as the callee of this method, otherwise <see langword="false" />.</param>
        /// <param name="pcrinfo">The component registration information.</param>
        /// <param name="fHostIsActivating">
        /// <see langword="true" /> if the host that is being activated, otherwise <see langword="false" />.</param>
        /// <param name="pchostinfo">The OLE host information.</param>
        /// <param name="dwReserved">Reserved.</param>
        void IOleComponent.OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating,
            OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        /// <summary>Gives the component a chance to do idle time tasks.  </summary>
        /// <param name="grfidlef">A set of flags indicating the type of idle tasks to perform, from <see cref="T:Microsoft.VisualStudio.OLE.Interop._OLEIDLEF" />.</param>
        /// <returns>
        /// <see langword="true" /> if more time is needed to perform the idle time tasks, <see langword="false" /> otherwise.</returns>
        int IOleComponent.FDoIdle(uint grfidlef)
        {
            return VSConstants.S_OK;
        }

        /// <summary>Called during each iteration of a message loop.</summary>
        /// <param name="uReason">The <see cref="T:Microsoft.VisualStudio.OLE.Interop.OLELOOP" /> representing the reason.</param>
        /// <param name="pMsgPeeked">The peeked message (from PeekMessage).</param>
        /// <param name="pvLoopData">The component data that was sent to <see cref="M:Microsoft.VisualStudio.OLE.Interop.IOleComponentManager.FPushMessageLoop(System.UInt32,System.UInt32,System.IntPtr)" />.</param>
        /// <returns>
        /// <see langword="true" /> if the message loop should continue, <see langword="false" /> otherwise. If <see langword="false" /> is returned, the component manager terminates the loop without removing <paramref name="pMsgPeeked" /> from the queue.</returns>
        int IOleComponent.FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when the component manager wishes to know if the component is in a state in which it can terminate.
        /// </summary>
        /// <param name="fPromptUser">
        /// <see langword="true" />
        /// If the user should be prompted, otherwise <see langword="false" />.
        /// </param>
        /// <returns>
        /// If <paramref name="fPromptUser" /> is <see langword="false" />, 
        /// the componentshould simply return <see langword="true" /> 
        /// if it can terminate, <see langword="false" /> otherwise. 
        /// If <paramref name="fPromptUser" /> is <see langword="true" />, 
        /// the component should return <see langword="true" /> if it can terminate without prompting the user. 
        /// Otherwise it should prompt the user, either asking the user if it can terminate and 
        /// returning <see langword="true" /> or <see langword="false" /> appropriately, 
        /// or giving an indication as to why it cannot terminate and returning <see langword="false" />.
        /// </returns>
        int IOleComponent.FQueryTerminate(int fPromptUser)
        {
            return 1; //true
        }

        /// <summary>Terminates the message loop.</summary>
        void IOleComponent.Terminate()
        {
        }

        /// <summary>Gets a window associated with the component.</summary>
        /// <param name="dwWhich">A value from <see cref="T:Microsoft.VisualStudio.OLE.Interop._OLECWINDOW" />.</param>
        /// <param name="dwReserved">Reserved for future use. Should be 0.</param>
        /// <returns>The HWND, or <see langword="null" /> if no such window exists.</returns>
        IntPtr IOleComponent.HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        /// <summary>Provides the document view to the document window.</summary>
        /// <param name="ppUnkDocView">[out] Pointer to the <see langword="IUnknown" /> interface of the document view. Used as an argument to <see cref="M:Microsoft.VisualStudio.Shell.Interop.IVsUIShell.CreateDocumentWindow(System.UInt32,System.String,Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy,System.UInt32,System.IntPtr,System.IntPtr,System.Guid@,System.String,System.Guid@,Microsoft.VisualStudio.OLE.Interop.IServiceProvider,System.String,System.String,System.Int32[],Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame@)" />.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        int IVsDeferredDocView.get_DocView(out IntPtr ppUnkDocView)
        {
            ppUnkDocView = Marshal.GetIUnknownForObject(this);
            return VSConstants.S_OK;
        }

        /// <summary>Retrieves the GUID for the pane or editor factory for later use when you create the view.</summary>
        /// <param name="pGuidCmdId">[out] Pointer to a GUID for the deferred view. Usually the GUID for the pane. Used as an argument to <see cref="M:Microsoft.VisualStudio.Shell.Interop.IVsUIShell.CreateDocumentWindow(System.UInt32,System.String,Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy,System.UInt32,System.IntPtr,System.IntPtr,System.Guid@,System.String,System.Guid@,Microsoft.VisualStudio.OLE.Interop.IServiceProvider,System.String,System.String,System.Int32[],Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame@)" /> when you create the view.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        int IVsDeferredDocView.get_CmdUIGuid(out Guid pGuidCmdId)
        {
            pGuidCmdId = FactoryGuid;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Indicates that the undo manager is blocking another undo manager from executing a linked action.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. 
        /// If it fails, it returns an error code.</returns>
        int IVsLinkedUndoClient.OnInterveningUnitBlockingLinkedUndo()
        {
            return VSConstants.E_FAIL;
        }

        /// <summary>
        /// Queries the status of the New Window command
        /// </summary>
        private static void OnQueryNewWindow(object sender, EventArgs e)
        {
            var command = (OleMenuCommand)sender;
            command.Enabled = true;
        }

        /// <summary>
        /// Executes the New Window command
        /// </summary>
        private void OnNewWindow(object sender, EventArgs e)
        {
            var uishellOpenDocument = (IVsUIShellOpenDocument)GetService(typeof(SVsUIShellOpenDocument));
            if (uishellOpenDocument == null) return;

            var windowFrameOrig = (IVsWindowFrame)GetService(typeof(SVsWindowFrame));
            if (windowFrameOrig == null) return;

            var logviewidPrimary = Guid.Empty;
            var hr = uishellOpenDocument.OpenCopyOfStandardEditor(windowFrameOrig, ref logviewidPrimary, out var windowFrameNew);
            if (windowFrameNew != null)
            {
                hr = windowFrameNew.Show();
            }
            ErrorHandler.ThrowOnFailure(hr);
        }

        /// <summary>
        /// Helper function used to add commands using IMenuCommandService
        /// </summary>
        /// <param name="mcs"> The IMenuCommandService interface.</param>
        /// <param name="menuGroup"> This guid represents the menu group of the command.</param>
        /// <param name="cmdId"> The command ID of the command.</param>
        /// <param name="commandEvent"> An EventHandler which will be called whenever the command is invoked.</param>
        /// <param name="queryEvent"> An EventHandler which will be called whenever we want to query the status of
        /// the command.  If null is passed in here then no EventHandler will be added.</param>
        private static void AddCommand(IMenuCommandService mcs, Guid menuGroup, int cmdId,
            EventHandler commandEvent, EventHandler queryEvent)
        {
            // Create the OleMenuCommand from the menu group, command ID, and command event
            var menuCommandId = new CommandID(menuGroup, cmdId);
            var command = new OleMenuCommand(commandEvent, menuCommandId);

            // Add an event handler to BeforeQueryStatus if one was passed in
            if (null != queryEvent)
            {
                command.BeforeQueryStatus += queryEvent;
            }

            // Add the command using our IMenuCommandService instance
            mcs.AddCommand(command);
        }

        /// <summary>
        /// Checks if the document is dirty or not
        /// </summary>
        /// <param name="pfIsDirty"></param>
        /// <returns></returns>
        protected virtual int OnIsDirty(out int pfIsDirty)
        {
            pfIsDirty = _isDirty ? 1 : 0;
            return VSConstants.S_OK;
        }

        /// <summary>Returns the unique identifier of the editor factory that created the IVsPersistDocData object.</summary>
        /// <param name="pClassId">[out] Pointer to the class identifier of the editor type.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        int IVsPersistDocData.GetGuidEditorType(out Guid pClassId)
        {
            pClassId = FactoryGuid;
            return VSConstants.S_OK;
        }

        /// <summary>Determines whether the document data has changed since the last save.</summary>
        /// <param name="pfDirty">[out] <see langword="true" /> if the document data has been changed.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        int IVsPersistDocData.IsDocDataDirty(out int pfDirty) =>
            ((IPersistFileFormat)this).IsDirty(out pfDirty);

        /// <summary>Sets the initial name (or path) for unsaved, newly created document data.</summary>
        /// <param name="pszDocDataPath">[in] String indicating the path of the document. Most editors can ignore this parameter. It exists for historical reasons.</param>
        /// <returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.</returns>
        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath) =>
            ((IPersistFileFormat)this).InitNew(FILE_FORMAT_INDEX);

        /// <summary>
        /// Loads the document data from the file specified.
        /// </summary>
        /// <param name="pszMkDocument">
        /// Path to the document file which needs to be loaded.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IVsPersistDocData.LoadDocData(string pszMkDocument)
        {
            return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
        }

        /// <summary>
        /// Saves the document data. Before actually saving the file, we first need to 
        /// indicate to the environment that a file is about to be saved. This is done 
        /// through the "SVsQueryEditQuerySave" service. We call the "QuerySaveFile" 
        /// function on the service instance and then proceed depending on the result 
        /// returned as follows:
        /// 
        /// If result is QSR_SaveOK - We go ahead and save the file and the file is not 
        /// read only at this point.
        /// 
        /// If result is QSR_ForceSaveAs - We invoke the "Save As" functionality which will 
        /// bring up the Save file name dialog.
        /// 
        /// If result is QSR_NoSave_Cancel - We cancel the save operation and indicate that 
        /// the document could not be saved by setting the "pfSaveCanceled" flag.
        /// 
        /// If result is QSR_NoSave_Continue - Nothing to do here as the file need not be 
        /// saved.
        /// </summary>
        /// <param name="dwSave">Flags which specify the file save options:
        /// VSSAVE_Save        - Saves the current file to itself.
        /// VSSAVE_SaveAs      - Prompts the User for a filename and saves the file to 
        ///                      the file specified.
        /// VSSAVE_SaveCopyAs  - Prompts the user for a filename and saves a copy of the 
        ///                      file with a name specified.
        /// VSSAVE_SilentSave  - Saves the file without prompting for a name or confirmation.  
        /// </param>
        /// <param name="pbstrMkDocumentNew">Pointer to the path to the new document.</param>
        /// <param name="pfSaveCanceled">Value 1 if the document could not be saved.</param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            pbstrMkDocumentNew = null;
            pfSaveCanceled = 0;
            int hr;

            switch (dwSave)
            {
                case VSSAVEFLAGS.VSSAVE_Save:
                case VSSAVEFLAGS.VSSAVE_SilentSave:
                    {
                        var queryEditQuerySave = (IVsQueryEditQuerySave2)GetService(typeof(SVsQueryEditQuerySave));
                        // --- Call QueryEditQuerySave
                        hr = queryEditQuerySave.QuerySaveFile(
                            FileName, // filename
                            0, // flags
                            null, // file attributes
                            out var result); // result

                        if (ErrorHandler.Failed(hr))
                        {
                            return hr;
                        }

                        // Process according to result from QuerySave
                        switch ((tagVSQuerySaveResult)result)
                        {
                            case tagVSQuerySaveResult.QSR_NoSave_Cancel:
                                // --- This is also case tagVSQuerySaveResult.QSR_NoSave_UserCanceled because these
                                // --- two tags have the same value.
                                pfSaveCanceled = ~0;
                                break;

                            case tagVSQuerySaveResult.QSR_SaveOK:
                                {
                                    // Call the shell to do the save for us
                                    hr = VsUiShell.SaveDocDataToFile(dwSave, this, FileName,
                                        out pbstrMkDocumentNew, out pfSaveCanceled);
                                    if (ErrorHandler.Failed(hr)) return hr;
                                }
                                break;

                            case tagVSQuerySaveResult.QSR_ForceSaveAs:
                                {
                                    // Call the shell to do the SaveAS for us
                                    hr = VsUiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, this, FileName,
                                        out pbstrMkDocumentNew, out pfSaveCanceled);
                                    if (ErrorHandler.Failed(hr)) return hr;
                                }
                                break;

                            case tagVSQuerySaveResult.QSR_NoSave_Continue:
                                // In this case there is nothing to do.
                                break;

                            default:
                                throw new COMException("Unsupported result from QEQS");
                        }
                        break;
                    }
                case VSSAVEFLAGS.VSSAVE_SaveAs:
                case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
                    {
                        // Make sure the file name as the right extension
                        if (String.Compare(FileExtensionUsed, Path.GetExtension(FileName), true, CultureInfo.CurrentCulture) != 0)
                        {
                            FileName += FileExtensionUsed;
                        }
                        // Call the shell to do the save for us
                        hr = VsUiShell.SaveDocDataToFile(dwSave, this, FileName, out pbstrMkDocumentNew,
                            out pfSaveCanceled);
                        if (ErrorHandler.Failed(hr)) return hr;
                        break;
                    }
                default:
                    throw new ArgumentException("Unsupported Save flag.");
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Close the IVsPersistDocData object.
        /// </summary>
        /// <returns>S_OK if the function succeeds.</returns>
        int IVsPersistDocData.Close() => VSConstants.S_OK;

        /// <summary>
        /// Called by the Running Document Table when it registers the document data. 
        /// </summary>
        /// <param name="docCookie">Handle for the document to be registered.</param>
        /// <param name="pHierNew">Pointer to the IVsHierarchy interface.</param>
        /// <param name="itemidNew">
        /// Item identifier of the document to be registered from VSITEM.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew) => VSConstants.S_OK;

        /// <summary>
        /// Renames the document data.
        /// </summary>
        /// <param name="grfAttribs">
        /// File attribute of the document data to be renamed. See the data type 
        /// __VSRDTATTRIB.
        /// </param>
        /// <param name="pHierNew">
        /// Pointer to the IVsHierarchy interface of the document being renamed.
        /// </param>
        /// <param name="itemidNew">
        /// Item identifier of the document being renamed. See the data type VSITEMID.
        /// </param>
        /// <param name="pszMkDocumentNew">Path to the document being renamed.</param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            FileName = pszMkDocumentNew;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Determines if it is possible to reload the document data.
        /// </summary>
        /// <param name="pfReloadable">set to 1 if the document can be reloaded.</param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
        {
            // --- Allow file to be reloaded
            pfReloadable = 1;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Reloads the document data.
        /// </summary>
        /// <param name="grfFlags">
        /// Flag indicating whether to ignore the next file change when reloading the 
        /// document data. This flag should not be set for us since we implement the 
        /// "IVsDocDataFileChangeControl" interface in order to indicate ignoring of file 
        /// changes.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IVsPersistDocData.ReloadDocData(uint grfFlags) =>
            ((IPersistFileFormat)this).Load(null, grfFlags, 0);

        /// <summary>
        /// Retrieves the class identifier (CLSID) of an object.
        /// </summary>
        /// <param name="pClassId">
        /// Class identifier of the object.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersist.GetClassID(out Guid pClassId)
        {
            pClassId = FactoryGuid;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Determines whether an object has changed since being saved to its current file.
        /// </summary>
        /// <param name="pfIsDirty">true if the document has changed.</param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersistFileFormat.IsDirty(out int pfIsDirty) => OnIsDirty(out pfIsDirty);

        /// <summary>
        /// Initialization for the object.
        /// </summary>
        /// <param name="nFormatIndex">
        /// Zero based index into the list of formats that indicates the current format
        /// of the file.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersistFileFormat.InitNew(uint nFormatIndex)
        {
            if (nFormatIndex != FILE_FORMAT_INDEX)
            {
                throw new ArgumentException("Unknown format");
            }
            // --- Until someone change the file, we can consider it not dirty as
            // --- the user would be annoyed if we prompt him to save an empty file
            _isDirty = false;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Loads the file content into the editor (into the controls representing the UI).
        /// </summary>
        /// <param name="pszFilename">
        /// Pointer to the full path name of the file to load.
        /// </param>
        /// <param name="grfMode">File format mode.</param>
        /// <param name="fReadOnly">
        /// determines if the file should be opened as read only.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            // --- A valid file name is required.
            if ((String.IsNullOrEmpty(pszFilename) && String.IsNullOrEmpty(FileName))
                || (!String.IsNullOrEmpty(pszFilename) && !File.Exists(pszFilename))) // pszFilename might not exist.
            {
                return VSConstants.E_INVALIDARG;
            }

            _loading = true;
            try
            {
                // --- If the new file name is null, then this operation is a reload
                var isReload = String.IsNullOrEmpty(pszFilename);

                // --- Show the wait cursor while loading the file
                VsUiShell.SetWaitCursor();

                // --- Set the new file name
                if (!isReload)
                {
                    // --- Unsubscribe from the notification of the changes in the previous file.
                    FileName = pszFilename;
                }
                // --- Load the file
                LoadFile(FileName);
                _isDirty = false;

                // --- Notify the load or reload
                NotifyDocChanged();
            }
            finally
            {
                _loading = false;
            }
            return VSConstants.S_OK;
        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// Use this method to sign that the content of the editor has been changed.
        /// </summary>
        // --------------------------------------------------------------------------------
        protected virtual void OnContentChanged()
        {
            // --- During the load operation the text of the control will change, but
            // --- this change must not be stored in the status of the document.
            // --- The only interesting case is when we are changing the document
            // --- for the first time.
            // Check if the QueryEditQuerySave service allow us to change the file
            if (_loading || _isDirty || !CanEditFile()) return;

            // --- It is possible to change the file, so update the status.
            _isDirty = true;
        }

        /// <summary>
        /// This function asks to the QueryEditQuerySave service if it is possible to
        /// edit the file.
        /// </summary>
        /// <returns>
        /// True if the editing of the file are enabled, otherwise returns false.
        /// </returns>
        private bool CanEditFile()
        {
            // --- Check the status of the recursion guard
            if (_gettingCheckoutStatus)
            {
                return false;
            }

            try
            {
                // Set the recursion guard
                _gettingCheckoutStatus = true;

                // Get the QueryEditQuerySave service
                var queryEditQuerySave = (IVsQueryEditQuerySave2)GetService(typeof(SVsQueryEditQuerySave));

                // Now call the QueryEdit method to find the edit status of this file
                string[] documents = { FileName };

                // This function can pop up a dialog to ask the user to checkout the file.
                // When this dialog is visible, it is possible to receive other request to change
                // the file and this is the reason for the recursion guard.
                var hr = queryEditQuerySave.QueryEditFiles(
                    0, // Flags
                    1, // Number of elements in the array
                    documents, // Files to edit
                    null, // Input flags
                    null, // Input array of VSQEQS_FILE_ATTRIBUTE_DATA
                    out var result, // result of the checkout
                    out _ // Additional flags
                );
                if (ErrorHandler.Succeeded(hr) && (result == (uint)tagVSQueryEditResult.QER_EditOK))
                {
                    // In this case (and only in this case) we can return true from this function.
                    return true;
                }
            }
            finally
            {
                _gettingCheckoutStatus = false;
            }
            return false;
        }

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName"></param>
        protected abstract void LoadFile(string fileName);

        /// <summary>
        /// Gets an instance of the RunningDocumentTable (RDT) service which manages the 
        /// set of currently open documents in the environment and then notifies the 
        /// client that an open document has changed.
        /// </summary>
        private void NotifyDocChanged()
        {
            // --- Make sure that we have a file name
            if (FileName.Length == 0)
            {
                return;
            }

            // --- Get a reference to the Running Document Table
            var runningDocTable = (IVsRunningDocumentTable)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsRunningDocumentTable));

            // --- Lock the document
            var hr = runningDocTable.FindAndLockDocument(
                // ReSharper disable UnusedVariable
                (uint)_VSRDTFLAGS.RDT_ReadLock,
                FileName,
                out var hierarchy,
                out var itemId,
                out var docData,
                out var docCookie
            // ReSharper restore UnusedVariable
            );
            ErrorHandler.ThrowOnFailure(hr);

            // --- Send the notification
            hr = runningDocTable.NotifyDocumentChanged(docCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);

            // --- Unlock the document.
            // --- We have to unlock the document even if the previous call failed.
            runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie);

            // --- Check Off the call to NotifyDocChanged failed.
            ErrorHandler.ThrowOnFailure(hr);
        }

        /// <summary>
        /// Save the contents of the editor into the specified file. If doing the save 
        /// on the same file, we need to suspend notifications for file changes during 
        /// the save operation.
        /// </summary>
        /// <param name="pszFilename">
        /// Pointer to the file name. If the pszFilename parameter is a null reference 
        /// we need to save using the current file.
        /// </param>
        /// <param name="fRemember">
        /// Boolean value that indicates whether the pszFileName parameter is to be used 
        /// as the current working file.
        /// If remember != 0, pszFileName needs to be made the current file and the 
        /// dirty flag needs to be cleared after the save. Also, file notifications need 
        /// to be enabled for the new file and disabled for the old file.
        /// If remember == 0, this save operation is a Save a Copy As operation. In this 
        /// case, the current file is unchanged and dirty flag is not cleared.
        /// </param>
        /// <param name="nFormatIndex">
        /// Zero based index into the list of formats that indicates the format in which 
        /// the file will be saved.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            // --- switch into the NoScribble mode
            _noScribbleMode = true;
            try
            {
                // --- If file is null or same --> SAVE
                if (pszFilename == null || pszFilename == FileName)
                {
                    SaveFile(FileName);
                    _isDirty = false;
                }
                else
                {
                    // --- If remember --> SaveAs 
                    if (fRemember != 0)
                    {
                        FileName = pszFilename;
                        SaveFile(FileName);
                        _isDirty = false;
                    }
                    else // --- Else, Save a Copy As
                    {
                        SaveFile(pszFilename);
                    }
                }
            }
            finally
            {
                // --- Switch into the Normal mode
                _noScribbleMode = false;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName"></param>
        public abstract void SaveFile(string fileName);

        /// <summary>
        /// Notifies the object that it has concluded the Save transaction.
        /// </summary>
        /// <param name="pszFilename">Pointer to the file name.</param>
        /// <returns>S_OK if the function succeeds.</returns>
        int IPersistFileFormat.SaveCompleted(string pszFilename)
            => _noScribbleMode ? VSConstants.S_FALSE : VSConstants.S_OK;

        /// <summary>
        /// Returns the path to the object's current working file.
        /// </summary>
        /// <param name="ppszFilename">Pointer to the file name.</param>
        /// <param name="pnFormatIndex">
        /// Value that indicates the current format of the file as a zero based index into 
        /// the list of formats. Since we support only a single format, we need to 
        /// return zero. Subsequently, we will return a single element in the format list 
        /// through a call to GetFormatList.
        /// </param>
        /// <returns>S_OK if the function succeeds.</returns>
        int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            // --- We only support 1 format so return its index
            pnFormatIndex = FILE_FORMAT_INDEX;
            ppszFilename = FileName;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Provides the caller with the information necessary to open the standard 
        /// common "Save As" dialog box. 
        /// </summary>
        /// <param name="ppszFormatList">
        /// Pointer to a string that contains pairs of format filter strings.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersistFileFormat.GetFormatList(out string ppszFormatList)
        {
            var formatList =
                string.Format(CultureInfo.CurrentCulture,
                    "Editor Files (*{0}){1}*{0}{1}{1}",
                    FileExtensionUsed, END_LINE_CHAR);
            ppszFormatList = formatList;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Retrieves the class identifier (CLSID) of an object.
        /// </summary>
        /// <param name="pClassId">
        /// Class identifier of the object.
        /// </param>
        /// <returns>S_OK if the method succeeds.</returns>
        int IPersistFileFormat.GetClassID(out Guid pClassId)
        {
            pClassId = FactoryGuid;
            return VSConstants.S_OK;
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
