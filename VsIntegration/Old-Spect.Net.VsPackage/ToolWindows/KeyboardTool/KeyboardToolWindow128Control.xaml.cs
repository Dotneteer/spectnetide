using System.Windows.Controls;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for KeyboardToolWindow128Control.xaml
    /// </summary>
    public partial class KeyboardToolWindow128Control : ISupportsMvvm<KeyboardToolViewModel>
    {
        private KeyPressHandler _keyPressHandler;

        /// <summary>
        /// Gets the view model instance
        /// </summary>
        public KeyboardToolViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        public void SetVm(KeyboardToolViewModel vm)
        {
            DataContext = Vm = vm;
            _keyPressHandler = new KeyPressHandler();
        }

        public KeyboardToolWindow128Control()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                SetupKeys(Row1);
                SetupKeys(Row2);
                SetupKeys(Row3);
                SetupKeys(Row4);
                SetupKeys(Row5);
                EnterKey.MainKeyClicked += _keyPressHandler.OnMainKeyClicked;
                EnterKey.KeyReleased += _keyPressHandler.OnKeyReleased;
            };

            Unloaded += (s, e) =>
            {
                ReleaseKeys(Row1);
                ReleaseKeys(Row2);
                ReleaseKeys(Row3);
                ReleaseKeys(Row4);
                ReleaseKeys(Row5);
                EnterKey.MainKeyClicked -= _keyPressHandler.OnMainKeyClicked;
                EnterKey.KeyReleased -= _keyPressHandler.OnKeyReleased;
            };
        }

        /// <summary>
        /// Sets up all keys to handle mouse events
        /// </summary>
        private void SetupKeys(Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Single128KeyControl key)
                {
                    key.MainKeyClicked += _keyPressHandler.OnMainKeyClicked;
                    key.SymShiftKeyClicked += _keyPressHandler.OnSymShiftKeyClicked;
                    key.ExtKeyClicked += _keyPressHandler.OnExtKeyClicked;
                    key.ExtShiftKeyClicked += _keyPressHandler.OnExtShiftKeyClicked;
                    key.NumericControlKeyClicked += _keyPressHandler.OnNumericControlKeyClicked;
                    key.GraphicsControlKeyClicked += _keyPressHandler.OnGraphicsControlKeyClicked;
                    key.NumericShiftKeyClicked += _keyPressHandler.OnNumericShiftKeyClicked;
                    key.KeyReleased += _keyPressHandler.OnKeyReleased;
                }
                else if (child is Wide128KeyControl wideKey)
                {
                    wideKey.MainKeyClicked += _keyPressHandler.OnMainKeyClicked;
                    wideKey.KeyReleased += _keyPressHandler.OnKeyReleased;
                }
            }
        }

        /// <summary>
        /// Unsubscribes from mouse events
        /// </summary>
        private void ReleaseKeys(Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Single128KeyControl key)
                {
                    key.MainKeyClicked -= _keyPressHandler.OnMainKeyClicked;
                    key.SymShiftKeyClicked -= _keyPressHandler.OnSymShiftKeyClicked;
                    key.ExtKeyClicked -= _keyPressHandler.OnExtKeyClicked;
                    key.ExtShiftKeyClicked -= _keyPressHandler.OnExtShiftKeyClicked;
                    key.NumericControlKeyClicked -= _keyPressHandler.OnNumericControlKeyClicked;
                    key.GraphicsControlKeyClicked -= _keyPressHandler.OnGraphicsControlKeyClicked;
                    key.NumericShiftKeyClicked += _keyPressHandler.OnNumericShiftKeyClicked;
                    key.KeyReleased -= _keyPressHandler.OnKeyReleased;
                }
                else if (child is Wide128KeyControl wideKey)
                {
                    wideKey.MainKeyClicked -= _keyPressHandler.OnMainKeyClicked;
                    wideKey.KeyReleased -= _keyPressHandler.OnKeyReleased;
                }
            }
        }
    }
}
