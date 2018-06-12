using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace SilverlightCustomControls
{
    public partial class EtapesControl : UserControl
    {
        private Binding bindingTotal;
        private Binding bindingEstime;
        private CurrencyConverter converter = new CurrencyConverter();
        
        public EtapesControl()
        {
            // THIS SETS THE FRAMEWORK ELEMENT'S CULTURE TO THE CURRENT CULTURE !! //
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

            bindingTotal = new Binding("Screen.ProjetProperty.Total");
            bindingTotal.Converter = converter;
            bindingTotal.Mode = BindingMode.OneWay;
            bindingTotal.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            bindingEstime = new Binding("Screen.ProjetEtapes.SelectedItem.Estime");
            bindingEstime.Converter = converter;
            bindingEstime.Mode = BindingMode.TwoWay;
            bindingEstime.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            InitializeComponent();

            textBoxValeur.SetBinding(TextBox.TextProperty, bindingTotal);
            textBoxEstime.SetBinding(TextBox.TextProperty, bindingEstime);
        }

        private void comboBoxEtapes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxEtapes.SelectedValue != null)
            {
                string etape = comboBoxEtapes.SelectedValue.ToString().ToUpper();

                SetEtape(etape);
            }
        }

        public void SetEtapeChanged(SelectionChangedEventHandler handler)
        {
            comboBoxEtapes.SelectionChanged -= handler;
            comboBoxEtapes.SelectionChanged += handler;
        }

        public ComboBox GetComboBox()
        {
            return comboBoxEtapes;
        }

        public void SetEtape(string etape)
        {
            if (etape.Equals("PRODUCTION"))
            {
                txtDate.Text = "Heures de fab:";
                datePickProd.Visibility = Visibility.Collapsed;
                datePickProd.IsEnabled = false;
                textBoxHeures.Visibility = Visibility.Visible;
                textBoxHeures.IsEnabled = true;
            }
            else if (etape.Equals("LIVRÉ"))
            {
                txtDate.Text = "Heures de fab:";
                datePickProd.Visibility = Visibility.Collapsed;
                datePickProd.IsEnabled = false;
                textBoxHeures.Visibility = Visibility.Visible;
                textBoxHeures.IsEnabled = false;
            }
            else if (etape.Equals("ANNULÉ") || etape.Equals("PERDU"))
            {
                textBoxValeur.IsEnabled = false;
                textBoxHeures.IsEnabled = false;
            }
            else
            {
                txtDate.Text = "Date prod.:";
                datePickProd.Visibility = Visibility.Visible;
                datePickProd.IsEnabled = true;
                textBoxHeures.Visibility = Visibility.Collapsed;
                textBoxHeures.IsEnabled = false;
            }
        }
    }
}
