using System.Collections;
using System.ComponentModel;

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
