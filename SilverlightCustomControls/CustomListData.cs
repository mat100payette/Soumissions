using System;
using System.Collections;
using System.Collections.Generic;
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
    public class CustomListData : INotifyPropertyChanged
    {
        private IList listProperty;

        public CustomListData() { }

        public IList CustomDataProperty
        {
            get { return listProperty; }
            set
            {
                listProperty = value;
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
