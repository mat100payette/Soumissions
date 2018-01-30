using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightCustomControls
{
    public partial class OptionsStats : UserControl
    {
        private ObservableCollection<string> affichees = new ObservableCollection<string>();
        private ObservableCollection<string> cachees = new ObservableCollection<string>();

        private CustomListData cAffichees = new CustomListData();
        private CustomListData cCachees = new CustomListData();

        private Binding bindAffichees = new Binding("CustomDataProperty");
        private Binding bindCachees = new Binding("CustomDataProperty");

        public OptionsStats()
        {
            cAffichees.CustomDataProperty = affichees;
            bindAffichees.Source = cAffichees;
            bindAffichees.Mode = BindingMode.TwoWay;

            cCachees.CustomDataProperty = cachees;
            bindCachees.Source = cCachees;
            bindCachees.Mode = BindingMode.TwoWay;

            InitializeComponent();

            listAffichees.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindAffichees);
            listCachees.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindCachees);
        }

        public void Init(List<string> newAffichees, List<string> newCachees)
        {
            affichees = new ObservableCollection<string>(newAffichees);
            cAffichees.CustomDataProperty = affichees;
            listAffichees.SetBinding(ItemsControl.ItemsSourceProperty, bindAffichees);

            cachees = new ObservableCollection<string>(newCachees);
            cCachees.CustomDataProperty = cachees;
            listCachees.SetBinding(ItemsControl.ItemsSourceProperty, bindCachees);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedEtapes = new List<string>();
            foreach (string etape in listCachees.SelectedItems)
            {
                selectedEtapes.Add(etape);
            }
            foreach (string etape in selectedEtapes)
            {
                cachees.Remove(etape);
                affichees.Add(etape);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedEtapes = new List<string>();
            foreach (string etape in listAffichees.SelectedItems)
            {
                selectedEtapes.Add(etape);
            }
            foreach (string etape in selectedEtapes)
            {
                affichees.Remove(etape);
                cachees.Add(etape);
            }
        }

        public void addOKHandler(RoutedEventHandler handler)
        {
            btnOK.Click -= handler;
            btnOK.Click += handler;
        }

        public Tuple<List<string>, List<string>> GetEtapes()
        {
            return new Tuple<List<string>, List<string>>(affichees.ToList(), cachees.ToList());
        }
    }
}
