using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SilverlightCustomControls
{
    public partial class ProjetFilterControl : UserControl
    {
        private ObservableCollection<string> selectedEtapes = new ObservableCollection<string>();
        private ObservableCollection<int> selectedVendeurs = new ObservableCollection<int>();
        private ObservableCollection<int> selectedIngenieurs = new ObservableCollection<int>();
        private ObservableCollection<int> selectedEntrepreneurs = new ObservableCollection<int>();
        private ObservableCollection<int> selectedDistributeurs = new ObservableCollection<int>();
        private ObservableCollection<string> allContacts = new ObservableCollection<string>();

        private CustomListData cEtapes = new CustomListData();
        private CustomListData cVendeurs = new CustomListData();
        private CustomListData cIngenieurs = new CustomListData();
        private CustomListData cEntrepreneurs = new CustomListData();
        private CustomListData cDistributeurs = new CustomListData();
        private CustomListData cAllContacts = new CustomListData();

        private Binding bindEtapes = new Binding("CustomDataProperty");
        private Binding bindVendeurs = new Binding("CustomDataProperty");
        private Binding bindIngenieurs = new Binding("CustomDataProperty");
        private Binding bindEntrepreneurs = new Binding("CustomDataProperty");
        private Binding bindDistributeurs = new Binding("CustomDataProperty");
        private Binding bindAllContacts = new Binding("CustomDataProperty");

        public ProjetFilterControl()
        {
            cEtapes.CustomDataProperty = selectedEtapes;
            bindEtapes.Source = cEtapes;
            bindEtapes.Mode = BindingMode.TwoWay;

            cVendeurs.CustomDataProperty = selectedVendeurs;
            bindVendeurs.Source = cVendeurs;
            bindVendeurs.Mode = BindingMode.TwoWay;

            cIngenieurs.CustomDataProperty = selectedIngenieurs;
            bindIngenieurs.Source = cIngenieurs;
            bindIngenieurs.Mode = BindingMode.TwoWay;

            cEntrepreneurs.CustomDataProperty = selectedEntrepreneurs;
            bindEntrepreneurs.Source = cEntrepreneurs;
            bindEntrepreneurs.Mode = BindingMode.TwoWay;

            cDistributeurs.CustomDataProperty = selectedDistributeurs;
            bindDistributeurs.Source = cDistributeurs;
            bindDistributeurs.Mode = BindingMode.TwoWay;

            cAllContacts.CustomDataProperty = allContacts;
            bindAllContacts.Source = cAllContacts;
            bindAllContacts.Mode = BindingMode.OneWay;
            bindAllContacts.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            InitializeComponent();

            listEtapes.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindEtapes);
            listVendeurs.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindVendeurs);
            listIngenieurs.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindIngenieurs);
            listEntrepreneurs.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindEntrepreneurs);
            listDistributeurs.SetBinding(SelectedValuesListBox.SelectedValuesProperty, bindDistributeurs);
        }

        public void SetContacts(IEnumerable<string> contacts)
        {
            if (allContacts != contacts)
            {
                allContacts = new ObservableCollection<string>(contacts);
                cAllContacts.CustomDataProperty = allContacts;
                listContacts.SetBinding(ItemsControl.ItemsSourceProperty, bindAllContacts);
            }
        }

        public string[] GetFilter()
        {
            string etapes = ";";
            foreach (var e in listEtapes.SelectedValues) { etapes += e + ";"; }
            string vendeurs = ";";
            foreach (var v in listVendeurs.SelectedValues) { vendeurs += v.ToString() + ";"; }
            string ingenieurs = ";";
            foreach (var i in listIngenieurs.SelectedValues) { ingenieurs += i.ToString() + ";"; }
            string entrepreneurs = ";";
            foreach (var e in listEntrepreneurs.SelectedValues) { entrepreneurs += e.ToString() + ";"; }
            string distributeurs = ";";
            foreach (var d in listDistributeurs.SelectedValues) { distributeurs += d.ToString() + ";"; }
            string contacts = ";";
            foreach (var c in listContacts.SelectedItems) { contacts += c + ";"; }
            string produit = textBoxProduit.Text;
            
            return new string[9] { textBoxPVA.Text, textBoxNom.Text, etapes, vendeurs, ingenieurs, entrepreneurs, distributeurs, contacts, produit };
        }

        public void addFilterHandler(RoutedEventHandler handler)
        {
            btnFiltrer.Click -= handler;
            btnFiltrer.Click += handler;
        }

        private void btnEffacer_Click(object sender, RoutedEventArgs e)
        {
            listEtapes.SelectedItems.Clear();
            listVendeurs.SelectedItems.Clear();
            listIngenieurs.SelectedItems.Clear();
            listEntrepreneurs.SelectedItems.Clear();
            listDistributeurs.SelectedItems.Clear();
            listContacts.SelectedItems.Clear();
            textBoxProduit.Text = string.Empty;
        }
    }
}
