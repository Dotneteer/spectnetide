using System.Windows;
using System.Windows.Input;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class provides helper functions for key handling
    /// </summary>
    public static class KeyHandlingHelper
    {
        /// <summary>
        /// This method handles the list view scrolling keys for the specified dependency object.
        /// </summary>
        /// <param name="element">Dependency object</param>
        /// <param name="e">Key event arguments</param>
        /// <remarks>
        /// Handled keys: Up, Down, (Ctrl+)PageUp, (Ctrl+)PageDown, Home, End
        /// </remarks>
        public static void HandleListViewKeyEvents(this DependencyObject element, KeyEventArgs e)
        {
            var stack = element.GetInnerStackPanel();
            if (stack == null) return;

            var sw = element.GetScrollViewer();
            if (sw == null) return;

            var ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var multiplier = ctrlPressed ? 10 : 1;
            switch (e.Key)
            {
                case Key.Up:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset - 1.0);
                    e.Handled = true;
                    break;

                case Key.Down:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset + 1.0);
                    e.Handled = true;
                    break;

                case Key.PageUp:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset - stack.ViewportHeight * multiplier);
                    e.Handled = true;
                    break;

                case Key.PageDown:
                    sw.ScrollToVerticalOffset(stack.VerticalOffset + stack.ViewportHeight * multiplier);
                    e.Handled = true;
                    break;

                case Key.Home:
                    if (ctrlPressed)
                    {
                        sw.ScrollToTop();
                        e.Handled = true;
                    }
                    break;

                case Key.End:
                    if (ctrlPressed)
                    {
                        sw.ScrollToBottom();
                        e.Handled = true;
                    }
                    break;
            }
        }

        public static void HandleDebugKeys(this SpectrumGenericToolWindowViewModel vm, KeyEventArgs args)
        {
            if (args.Key == Key.F5)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Run in Debug mode
                    args.Handled = true;
                    vm.MachineViewModel.StartDebugVm();
                } else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    args.Handled = true;
                    vm.MachineViewModel.Start();
                }
            }
            else
            {
                if (!vm.VmPaused) return;

                if (args.Key == Key.F11 && Keyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step into
                    args.Handled = true;
                    vm.MachineViewModel.StepInto();
                }
                else if (args.Key == Key.System && args.SystemKey == Key.F10 && Keyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step over
                    args.Handled = true;
                    vm.MachineViewModel.StepOver();
                }
            }

            if (args.Handled)
            {
                SpectNetPackage.UpdateCommandUi();
            }
        }
    }
}