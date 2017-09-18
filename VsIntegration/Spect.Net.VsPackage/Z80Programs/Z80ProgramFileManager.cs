using System;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class is responsible for managing Z80 program files
    /// </summary>
    public class Z80ProgramFileManager
    {
        /// <summary>
        /// The hierarchy information of the associated item
        /// </summary>
        public IVsHierarchy Hierarchy { get; }

        /// <summary>
        /// The Id information of the associated item
        /// </summary>
        public uint ItemId { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80ProgramFileManager(IVsHierarchy hierarchy, uint itemId)
        {
            Hierarchy = hierarchy;
            ItemId = itemId;
        }

        /// <summary>
        /// The error list
        /// </summary>
        public ErrorListWindow ErrorList => VsxPackage.GetPackage<SpectNetPackage>().ErrorList;

        /// <summary>
        /// The full path of the item behind this Z80 program file
        /// </summary>
        protected string ItemPath
        {
            get
            {
                var singleItem = SpectNetPackage.IsSingleProjectItemSelection(out var hierarchy, out var itemId);
                if (!singleItem) return null;

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (!(hierarchy is IVsProject project)) return null;

                project.GetMkDocument(itemId, out var itemFullPath);
                return itemFullPath;
            }
        }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        /// <returns></returns>
        public bool Compile()
        {
            ErrorList.Clear();

            var code = File.ReadAllText(ItemPath);
            var compiler = new Z80Assembler();
            var output = compiler.Compile(code);

            if (output.ErrorCount == 0)
            {
                return true;
            }

            foreach (var error in output.Errors)
            {
                var errorTask = new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = TaskErrorCategory.Error,
                    HierarchyItem = Hierarchy,
                    Document = ItemPath,
                    Line = error.SourceLine,
                    Column = error.Position,
                    Text = error.Message
                };
                errorTask.Navigate += ErrorTaskOnNavigate;
                ErrorList.AddErrorTask(errorTask);
            }

            return false;
        }

        private void ErrorTaskOnNavigate(object sender, EventArgs eventArgs)
        {
            var task = sender as ErrorTask;
            if (task != null)
            {
                // TODO: navigate!
            }
        }
    }
}