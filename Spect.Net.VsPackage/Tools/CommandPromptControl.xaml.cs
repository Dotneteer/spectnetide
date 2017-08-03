using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Spect.Net.VsPackage.Tools
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

        /// <summary>
        /// This event is raised when the user presses the enter key
        /// </summary>
        public EventHandler<string> CommandLineEntered;

        public EventHandler<CancelEventArgs> CommandLineChanged;

        public CommandPromptControl()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Prompt = ">";
                CommandText = "G4567";
            }
        }

        private void OnCommandLineKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommandLineEntered?.Invoke(this, CommandLine.Text);
            }
        }
    }
}
