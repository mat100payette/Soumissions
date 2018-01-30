using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SilverlightCustomControls
{
    public partial class ProduitEditor : UserControl
    {
        private const string EMPTY_TEXT = "";

        private List<object> EMPTY_LIST = new List<object>();
        private ObservableCollection<string> selectedInsertions = new ObservableCollection<string>();
        private ObservableCollection<string> selectedOptions = new ObservableCollection<string>();

        private CustomListData optionsBC = new CustomListData();
        private CustomListData insertionsBC = new CustomListData();
        private CustomListData emptyList = new CustomListData();
        private CustomStringData emptyString = new CustomStringData();

        private Binding bindingEmptyList = new Binding("CustomDataProperty");
        private Binding bindingAjouts = new Binding("Screen.AjoutsBC");
        private Binding bindingInsertions = new Binding("CustomDataProperty");
        private Binding bindingOptions = new Binding("CustomDataProperty");

        private Binding bindingEmptyString = new Binding("CustomDataProperty");

        private Binding bindingSelectedItem = new Binding("Screen.ProjetProduits.SelectedItem.Produit.Item");
        private Binding bindingDesc = new Binding("Screen.ProjetProduits.SelectedItem.Description");
        private Binding bindingTag = new Binding("Screen.ProjetProduits.SelectedItem.Tag");

        private CultureInfo cultureInfo;
        private NumberStyles style;

        private SolidColorBrush RED = new SolidColorBrush(Colors.Red);
        private SolidColorBrush BLACK = new SolidColorBrush(Colors.Black);
        private Brush BORDER;

        private FrameworkElement focusElement;

        public ProduitEditor()
        {
            // THIS SETS THE FRAMEWORK ELEMENT'S CULTURE TO THE CURRENT CULTURE !! //
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

            // Empty List Binding
            emptyList.CustomDataProperty = EMPTY_LIST;
            bindingEmptyList.Source = emptyList;
            bindingEmptyList.Mode = BindingMode.OneWay;

            // Empty String binding
            emptyString.CustomDataProperty = string.Empty;
            bindingEmptyString.Source = emptyString;
            bindingEmptyString.Mode = BindingMode.OneWay;

            // Selected BC insertions binding
            insertionsBC.CustomDataProperty = selectedInsertions;
            bindingInsertions.Source = insertionsBC;
            bindingInsertions.Mode = BindingMode.TwoWay;
            bindingInsertions.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            // Selected Options binding
            optionsBC.CustomDataProperty = selectedOptions;
            bindingOptions.Source = optionsBC;
            bindingOptions.Mode = BindingMode.TwoWay;
            bindingOptions.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            
            // AjoutsBC binding
            bindingAjouts.Mode = BindingMode.OneWay;

            // Items binding
            bindingSelectedItem.Mode = BindingMode.TwoWay;
            bindingSelectedItem.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            
            // Description Binding
            bindingDesc.Mode = BindingMode.TwoWay;
            bindingDesc.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            // Description Binding
            bindingTag.Mode = BindingMode.TwoWay;
            bindingTag.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            cultureInfo = Thread.CurrentThread.CurrentCulture;
            style = (NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol) ^ NumberStyles.AllowThousands;

            InitializeComponent();
            
            listBoxOptions.SetBinding(ItemsControl.ItemsSourceProperty, bindingOptions);
            listBoxInsertions.SetBinding(ItemsControl.ItemsSourceProperty, bindingInsertions);
            AutoCompleteItem.SetBinding(AutoCompleteBox.SelectedItemProperty, bindingSelectedItem);
            txtDescription.SetBinding(TextBox.TextProperty, bindingDesc);
            txtTag.SetBinding(TextBox.TextProperty, bindingTag);

            checkBC();

            BORDER = AutoCompleteModele.BorderBrush;
        }

        public void clearFields()
        {
            AutoCompleteModele.Text = "";
            AutoCompleteItem.Text = "";
            TextBoxAutre.Text = "";
            txtDescription.Text = "";
            txtTag.Text = "";
            AutoCompleteModele.BorderBrush = BLACK;
        }

        private void reinitBC()
        {
            listBoxAjout.SelectedItem = null;
            AutoCompleteInsertions.Text = "";
            AutoCompleteOptions.Text = "";
        }

        public List<string> getInsertionsBC()
        {
            List<string> insertions = new List<string>();
            foreach (string insertion in listBoxInsertions.Items)
            {
                insertions.Add("" + insertion);
            }
            return insertions;
        }

        public void setInsertionsBC(List<string> insertions)
        {
            selectedInsertions.Clear();
            foreach (string insertion in insertions)
            {
                selectedInsertions.Add(insertion);
            }
        }

        public List<string> getOptionsBC()
        {
            List<string> options = new List<string>();
            foreach (string option in listBoxOptions.Items)
            {
                options.Add("" + option);
            }
            return options;
        }

        public void setOptionsBC(List<string> options)
        {
            selectedOptions.Clear();
            foreach (string option in options)
            {
                selectedOptions.Add(option);
            }
        }

        private void AutoCompleteModele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(AutoCompleteModele.SelectedItem);
            checkBC();
        }

        private void AutoCompleteModele_TextChanged(object sender, RoutedEventArgs e)
        {
            updateAutocompleteTexts(sender);
        }

        private void AutoCompleteItem_TextChanged(object sender, RoutedEventArgs e)
        {
            updateAutocompleteTexts(sender);
        }

        private void TextBoxAutre_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateAutocompleteTexts(sender);
        }

        private void updateAutocompleteTexts(object sender)
        {
            if (focusElement != null)
            {
                if (focusElement.Equals((FrameworkElement)sender))
                {

                    switch (sender.GetType().Name)
                    {
                        case "AutoCompleteBox":
                            if (!((AutoCompleteBox)sender).Text.Equals(string.Empty))
                            {
                                if (((AutoCompleteBox)sender).Name.Equals(AutoCompleteModele.Name))
                                {
                                    AutoCompleteItem.Text = string.Empty;
                                    TextBoxAutre.Text = string.Empty;
                                    checkBC();
                                }
                                else
                                {
                                    AutoCompleteModele.Text = string.Empty;
                                    TextBoxAutre.Text = string.Empty;
                                }
                            }
                            break;
                        case "TextBox":
                            if (!((TextBox)sender).Text.Equals(string.Empty))
                            {
                                AutoCompleteModele.Text = string.Empty;
                                AutoCompleteItem.Text = string.Empty;
                            }
                            break;
                    }
                }
            }
        }

        private void checkBC()
        {
            if (AutoCompleteModele.SelectedItem != null)
            {
                if (AutoCompleteModele.SelectedItem.ToString().ToLower().Contains("bc "))
                {
                    setBCComponents(true);
                } else
                {
                    setBCComponents(false);
                }
            }
            else
            {
                setBCComponents(false);
            }
        }

        private void setBCComponents(bool active)
        {
            if (active)
            {
                listBoxAjout.SetBinding(ItemsControl.ItemsSourceProperty, bindingAjouts);
                AutoCompleteInsertions.IsEnabled = true;
                AutoCompleteOptions.IsEnabled = true;
                listBoxInsertions.SetBinding(ItemsControl.ItemsSourceProperty, bindingInsertions);
                btnAdd.IsEnabled = true;
                btnRemove.IsEnabled = true;
                btnAddOption.IsEnabled = true;
                btnRemoveOption.IsEnabled = true;
                listBoxOptions.SetBinding(ItemsControl.ItemsSourceProperty, bindingOptions);
            }
            else
            {
                listBoxAjout.SetBinding(ItemsControl.ItemsSourceProperty, bindingEmptyList);
                AutoCompleteInsertions.IsEnabled = false;
                AutoCompleteOptions.IsEnabled = false;
                listBoxInsertions.SetBinding(ItemsControl.ItemsSourceProperty, bindingEmptyList);
                btnAdd.IsEnabled = false;
                btnRemove.IsEnabled = false;
                btnAddOption.IsEnabled = false;
                btnRemoveOption.IsEnabled = false;
                listBoxOptions.SetBinding(ItemsControl.ItemsSourceProperty, bindingEmptyList);
                reinitBC();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AutoCompleteInsertions.SelectedItem != null)
            {
                selectedInsertions.Add("" + AutoCompleteInsertions.SelectedItem);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxInsertions.SelectedItem != null)
            {
                selectedInsertions.Remove("" + listBoxInsertions.SelectedItem);
            }
        }

        private void btnAddOption_Click(object sender, RoutedEventArgs e)
        {
            if (AutoCompleteOptions.SelectedItem != null)
            {
                selectedOptions.Add("" + AutoCompleteOptions.SelectedItem);
            }
        }

        private void btnRemoveOption_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxOptions.SelectedItem != null)
            {
                selectedOptions.Remove("" + listBoxOptions.SelectedItem);
            }
        }

        public void updateAll()
        {
            txtQuantite.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtPrix.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            listBoxAjout.GetBindingExpression(Selector.SelectedItemProperty).UpdateSource();
            AutoCompleteModele.GetBindingExpression(AutoCompleteBox.SelectedItemProperty).UpdateSource();
            AutoCompleteItem.GetBindingExpression(AutoCompleteBox.SelectedItemProperty).UpdateSource();
            TextBoxAutre.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (!listBoxInsertions.GetBindingExpression(ItemsControl.ItemsSourceProperty).ParentBinding.Equals(bindingEmptyList))
            {
                listBoxInsertions.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateSource();
            }
            if (!listBoxOptions.GetBindingExpression(ItemsControl.ItemsSourceProperty).ParentBinding.Equals(bindingEmptyList))
            {
                listBoxOptions.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateSource();
            }
            txtDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtTag.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void txtPrixQte_TextChanged(object sender, TextChangedEventArgs e)
        {
            int qte;
            decimal prix;

            if (int.TryParse(txtQuantite.Text, out qte) && decimal.TryParse(txtPrix.Text.Replace(" ", ""), style, cultureInfo, out prix))
            {
                txtTotal.Foreground = BLACK;
                txtTotal.Text = "" + (qte * prix);
            }
            else
            {
                txtTotal.Foreground = RED;
                txtTotal.Text = "- $";
            }
        }

        private void totalLoaded(object sender, RoutedEventArgs e)
        {
            txtPrixQte_TextChanged(sender, null);
        }

        public bool validate()
        {
            if (AutoCompleteModele.SelectedItem == null && AutoCompleteItem.SelectedItem == null && TextBoxAutre.Text.Equals(""))
            {
                AutoCompleteModele.BorderBrush = RED;
                return false;
            }

            AutoCompleteModele.BorderBrush = BORDER;
            return true;
        }

        private void AutoCompleteModele_GotFocus(object sender, RoutedEventArgs e)
        {
            focusElement = (FrameworkElement)sender;
        }

        private void AutoCompleteItem_GotFocus(object sender, RoutedEventArgs e)
        {
            focusElement = (FrameworkElement)sender;
        }

        private void TextBoxAutre_GotFocus(object sender, RoutedEventArgs e)
        {
            focusElement = (FrameworkElement)sender;
        }

        public void resetColors()
        {
            AutoCompleteModele.BorderBrush = BORDER;
        }

        private void AutoCompleteModele_LostFocus(object sender, RoutedEventArgs e)
        {
            checkBC();
        }
    }
}
