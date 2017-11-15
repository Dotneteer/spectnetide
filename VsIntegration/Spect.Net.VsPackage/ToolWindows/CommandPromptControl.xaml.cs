﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// Interaction logic for CommandPromptControl.xaml
    /// </summary>
    public partial class CommandPromptControl
    {
        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(CommandPromptControl), new PropertyMetadata(">"));

        public string Prompt
        {
            get => (string)GetValue(PromptProperty);
            set => SetValue(PromptProperty, value);
        }

        public static readonly DependencyProperty CommandTextProperty = DependencyProperty.Register(
            "CommandText", typeof(string), typeof(CommandPromptControl), new PropertyMetadata(default(string)));

        public string CommandText
        {
            get => (string)GetValue(CommandTextProperty);
            set => SetValue(CommandTextProperty, value);
        }

        public static readonly DependencyProperty HelpUrlProperty = DependencyProperty.Register(
            "HelpUrl", typeof(string), typeof(CommandPromptControl), new PropertyMetadata(default(string)));

        public string HelpUrl
        {
            get => (string)GetValue(HelpUrlProperty);
            set => SetValue(HelpUrlProperty, value);
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength", typeof(int), typeof(CommandPromptControl), new PropertyMetadata(32));

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(
            "IsValid", typeof(bool), typeof(CommandPromptControl), new PropertyMetadata(true));

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public static readonly DependencyProperty ValidationMessageProperty = DependencyProperty.Register(
            "ValidationMessage", typeof(string), typeof(CommandPromptControl), new PropertyMetadata(default(string)));

        public string ValidationMessage
        {
            get => (string)GetValue(ValidationMessageProperty);
            set => SetValue(ValidationMessageProperty, value);
        }

        /// <summary>
        /// This event is raised when the user presses the enter key
        /// </summary>
        public EventHandler<CommandLineEventArgs> CommandLineEntered;

        /// <summary>
        /// This event is raised to preview the changes in the command line
        /// text input
        /// </summary>
        public EventHandler<TextCompositionEventArgs> PreviewCommandLineInput;

        public CommandPromptControl()
        {
            InitializeComponent();
            ValidationMessage = "";
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Prompt = ">";
                CommandText = "G4567";
            }
        }

        private void OnCommandLineKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var args = new CommandLineEventArgs(CommandText);
            CommandLineEntered?.Invoke(this, args);
            if (args.Handled)
            {
                CommandText = null;
            }
            if (args.Invalid)
            {
                IsValid = false;
            }
            e.Handled = true;
        }

        private void OnCommandLinePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == null || e.Text == "\r") return;
            PreviewCommandLineInput?.Invoke(sender, e);
        }

        private void OnCommandLinePreviewKeyDown(object sender, KeyEventArgs e)
        {
            IsValid = true;
        }

        private void PromptClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (!(Package.GetGlobalService(typeof(IVsWebBrowsingService)) is IVsWebBrowsingService service)) return;
            var url = $"{SpectNetPackage.COMMANDS_BASE_URL}/{HelpUrl}";
            service.Navigate(url, (uint)__VSWBNAVIGATEFLAGS.VSNWB_AddToMRU, out var ppFrame);
        }
    }
}
