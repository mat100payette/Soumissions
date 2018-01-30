using System;
using System.Windows.Controls;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;

namespace LightSwitchApplication.UserCode
{
    public class ModalWindow
    {
        private IVisualCollection _collection;
        private IVisualCollection _initCollection;
        private string _dialogName;
        private string _entityName;
        private IScreenObject _screen;
        private IContentItemProxy _window;
        private IEntityObject _entity;

        public ModalWindow(IVisualCollection visualCollection, string dialogName, string entityName = "")
        {
            _collection = visualCollection;
            _initCollection = visualCollection;
            _dialogName = dialogName;
            _entityName = ((entityName != "") ? entityName : _collection.Details.GetModel().ElementType.Name);
            _screen = _collection.Screen;
        }

        public void Initialise()
        {
            _window = _screen.FindControl(_dialogName);

            _window.ControlAvailable += (object s, ControlAvailableEventArgs e) => {
                var window = (ChildWindow)e.Control;

                window.Closed += (object s1, EventArgs e1) => {
                    DialogClosed(s1);
                };
            };
        }

        public void setEntityName(string name)
        {
            _entityName = name;
        }

        public bool CanAdd()
        {
            return (_collection.CanAddNew == true);
        }

        public bool CanView()
        {
            return (_collection.SelectedItem != null);
        }

        public void AddEntity()
        {
            _window.DisplayName = string.Format("Ajouter {0}", _entityName);

            OpenModalWindow();
        }

        public void ViewEntity()
        {
            _window.DisplayName = string.Format("{0}", _entityName);

            OpenModalWindow();
        }

        private void OpenModalWindow()
        {
            _collection = _initCollection;
            _entity = _collection.SelectedItem as IEntityObject;
            _screen.OpenModalWindow(_dialogName);
        }

        public void DialogOk()
        {
            if (_entity != null)
            {
                _screen.CloseModalWindow(_dialogName);
            }
        }

        public void DialogCancel()
        {
            if (_entity != null)
            {
                _screen.CloseModalWindow(_dialogName);

                DiscardChanges();
            }
        }

        public void DialogClosed(object sender)
        {
            var window = (ChildWindow)sender;

            if (window.DialogResult.HasValue == false)
            {
                DiscardChanges();
            }
        }

        private void DiscardChanges()
        {
            if (_entity != null)
            {
                _entity.Details.DiscardChanges();
            }
        }
    }
}
