using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightCustomControls
{
    public class CustomStringData : INotifyPropertyChanged
    {
        private string stringProperty;

        public CustomStringData() { }

        public string CustomDataProperty
        {
            get { return stringProperty; }
            set
            {
                stringProperty = value;
                OnPropertyChanged("CustomDataProperty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
