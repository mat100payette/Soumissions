using System;
using System.Windows.Controls;
using Microsoft.LightSwitch.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.ComponentModel;

namespace LightSwitchApplication.UserCode
{
    public class ModalWindow
    {
        private bool _initialized = false;
        private string _controlName;
        private string _windowName;
        private IScreenObject _screen;
        private IContentItemProxy _window;
        private EventHandler _onClosed;
        private EventHandler<CancelEventArgs> _onClosing;

        public ModalWindow(IScreenObject screen, string controlName, string windowName = "", 
            EventHandler onClosed = null, EventHandler<CancelEventArgs> onClosing = null)
        {
            _controlName = controlName;
            _windowName = windowName;
            _screen = screen;
            _onClosed = onClosed;
            _onClosing = onClosing;

            Initialise();
        }

        private void Initialise()
        {
            _window = _screen.FindControl(_controlName);

            _window.ControlAvailable += (object s, ControlAvailableEventArgs e) => {
                if (!_initialized)
                {
                    var window = (ChildWindow)e.Control;

                    if (_onClosed != null)
                    {
                        window.Closed -= _onClosed;
                        window.Closed += _onClosed;
                    }

                    if (_onClosing != null)
                    {
                        window.Closing -= _onClosing;
                        window.Closing += _onClosing;
                    }
                }

                _initialized = true;
            };
        }

        public void setWindowName(string name)
        {
            _windowName = name;
        }

        public void OpenModalWindow()
        {
            _screen.OpenModalWindow(_controlName);
        }

        public void CloseModalWindow()
        {
            _screen.CloseModalWindow(_controlName);
        }
    }
}
