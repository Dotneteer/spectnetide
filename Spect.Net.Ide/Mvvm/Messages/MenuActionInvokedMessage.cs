using System;
using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.Ide.Mvvm.Messages
{
    /// <summary>
    /// This message instructs the navigation system that a menu action has been invoked
    /// </summary>
    public class MenuActionInvokedMessage : MessageBase
    {
        public Type ViewModelType { get; }

        public Action<object> ImmediateAction { get; }

        public object Tag { get; }

        public MenuActionInvokedMessage(Type viewModelType, object tag = null)
        {
            ViewModelType = viewModelType;
            ImmediateAction = null;
            Tag = tag;
        }

        public MenuActionInvokedMessage(Action<object> immediateAction, object tag = null)
        {
            ViewModelType = null;
            ImmediateAction = immediateAction;
            Tag = tag;
        }
    }
}