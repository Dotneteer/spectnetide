using System;
using Spect.Net.Ide.Mvvm.Navigation;

namespace Spect.Net.Ide.ViewModels
{
    /// <summary>
    /// The common view model for all panes that can be placed somewhere in the IDE
    /// </summary>
    public class PaneViewModel: ViewModelBaseWithDesignTimeFix
    {
        private string _contentId;
        private string _title;
        private Uri _iconSource;
        private bool _isSelected;
        private bool _isActive;

        /// <summary>
        /// The unique ID of a particular pane
        /// </summary>
        public string ContentId
        {
            get { return _contentId; }
            set { Set(ref _contentId, value); }
        }

        /// <summary>
        /// Pane title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        /// <summary>
        /// Pane icon
        /// </summary>
        public virtual Uri IconSource
        {
            get { return _iconSource; }
            set { Set(ref _iconSource, value); }
        }

        /// <summary>
        /// Indicates if the pane is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        /// <summary>
        /// Indicates if the pane is active
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { Set(ref _isActive, value); }
        }
    }
}