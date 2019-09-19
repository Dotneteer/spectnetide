using System.Windows;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.VsxLibrary;
using WindowsKeyboard = System.Windows.Input.Keyboard;

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

            var ctrlPressed = WindowsKeyboard.IsKeyDown(Key.LeftCtrl) || WindowsKeyboard.IsKeyDown(Key.RightCtrl);
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

        public static async void HandleDebugKeys(this SpectrumGenericToolWindowViewModel vm, KeyEventArgs args)
        {
            var machine = vm.EmulatorViewModel.Machine;
            var state = vm.EmulatorViewModel.MachineState;
            if (args.Key == Key.F5)
            {
                if (WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Run in Debug mode
                    args.Handled = true;
                    machine.StartDebug();
                } else if (WindowsKeyboard.Modifiers == ModifierKeys.Control)
                {
                    args.Handled = true;
                    machine.Start();
                }
            }
            else
            {
                if (state != VmState.Paused) return;

                if (args.Key == Key.F11 && WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step into
                    args.Handled = true;
                    await machine.StepInto();
                }
                else if (args.Key == Key.System && args.SystemKey == Key.F10 && WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step over
                    args.Handled = true;
                    await machine.StepOver();
                }
                else if (args.Key == Key.F12 && WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step over
                    args.Handled = true;
                    await machine.StepOut();
                }
            }

            if (args.Handled)
            {
                VsxAsyncPackage.UpdateCommandUi();
            }
        }

        public static async void HandleDebugKeys(this SpectrumToolWindowViewModelBase vm, KeyEventArgs args)
        {
            var machine = vm.EmulatorViewModel.Machine;
            var state = vm.EmulatorViewModel.MachineState;
            if (args.Key == Key.F5)
            {
                if (WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Run in Debug mode
                    args.Handled = true;
                    machine.StartDebug();
                }
                else if (WindowsKeyboard.Modifiers == ModifierKeys.Control)
                {
                    args.Handled = true;
                    machine.Start();
                }
            }
            else
            {
                if (state != VmState.Paused) return;

                if (args.Key == Key.F11 && WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step into
                    args.Handled = true;
                    await machine.StepInto();
                }
                else if (args.Key == Key.System && args.SystemKey == Key.F10 && WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step over
                    args.Handled = true;
                    await machine.StepOver();
                }
                else if (args.Key == Key.F12 && WindowsKeyboard.Modifiers == ModifierKeys.None)
                {
                    // --- Step over
                    args.Handled = true;
                    await machine.StepOut();
                }
            }

            if (args.Handled)
            {
                VsxAsyncPackage.UpdateCommandUi();
            }
        }
    }
}