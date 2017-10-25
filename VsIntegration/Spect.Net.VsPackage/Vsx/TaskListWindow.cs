using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.Vsx
{
    public class TaskListWindow
    {
        private readonly TaskProvider _taskProvider;

        public TaskListWindow()
        {
            _taskProvider = new TaskProvider(VsxPackage.GetPackage<SpectNetPackage>());
        }

        /// <summary>
        /// Clears the content of the Error List
        /// </summary>
        public void Clear()
        {
            _taskProvider.Tasks.Clear();
        }

        /// <summary>
        /// Adds the specified error to the provider
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(Microsoft.VisualStudio.Shell.Task task)
        {
            _taskProvider.Tasks.Add(task);
        }

        /// <summary>
        /// Navigates to the source code associated with the specified task
        /// </summary>
        /// <param name="task">Error task</param>
        public void Navigate(Microsoft.VisualStudio.Shell.Task task)
        {
            _taskProvider.Navigate(task, VSConstants.LOGVIEWID_Primary);
        }
    }
}