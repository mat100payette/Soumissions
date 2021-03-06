﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SilverlightCustomControls
{
    public partial class ProduitEditor : UserControl
    {
        private ObservableCollection<string> selectedInsertions = new ObservableCollection<string>();
        private ObservableCollection<string> selectedOptions = new ObservableCollection<string>();

        private CustomListData optionsBC = new CustomListData();
        private CustomListData insertionsBC = new CustomListData();
        
        private Binding bindingAjouts = new Binding("Screen.AjoutsBC");
        private Binding bindingInsertions = new Binding("CustomDataProperty");
        private Binding bindingOptions = new Binding("CustomDataProperty");

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

            // Selected BC insertions binding
            insertionsBC.CustomDataProperty = selectedInsertions;
            bindingInsertions.Source = insertionsBC;
            bindingInsertions.Mode = BindingMode.TwoWay;
            bindingInsertions.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            // Selected Options binding
            optionsBC.CustomDataProperty = selectedOptions;
            bindingOptions.Source = optionsBC;
            bindingOptions.Mode = BindingMode.TwoWay;
            bindingOptions.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            
            // AjoutsBC binding
            bindingAjouts.Mode = BindingMode.OneWay;

            cultureInfo = Thread.CurrentThread.CurrentCulture;
            style = (NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol) ^ NumberStyles.AllowThousands;

            InitializeComponent();

            listBoxAjout.SetBinding(ItemsControl.ItemsSourceProperty, bindingAjouts);
            listBoxInsertions.SetBinding(ItemsControl.ItemsSourceProperty, bindingInsertions);
            listBoxOptions.SetBinding(ItemsControl.ItemsSourceProperty, bindingOptions);

            CheckBC();

            BORDER = AutoCompleteModele.BorderBrush;
        }

        public void Clear()
        {
            AutoCompleteModele.SelectedItem = null;
            AutoCompleteModele.Text = string.Empty;
            AutoCompleteItem.SelectedItem = null;
            AutoCompleteItem.Text = string.Empty;
            TextBoxAutre.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtPrix.Text = string.Empty;
            txtQuantite.Text = string.Empty;
        }

        private void ReinitBC()
        {
            listBoxAjout.SelectedItem = null;
            AutoCompleteInsertions.Text = string.Empty;
            AutoCompleteOptions.Text = string.Empty;
            selectedInsertions.Clear();
            selectedOptions.Clear();
        }

        /**
         *      Modele, Item, Autre, AjoutBC, InsertionsBC, OptionsBC
         **/
        public Tuple<string, string, string, string, List<string>, List<string>> GetProduit()
        {
            return new Tuple<string, string, string, string, List<string>, List<string>>(
                    AutoCompleteModele.Text, AutoCompleteItem.Text, TextBoxAutre.Text,
                    listBoxAjout.SelectedItem == null ? string.Empty : listBoxAjout.SelectedItem.ToString(),
                    GetInsertionsBC(), GetOptionsBC()
                );
        }

        public void SetProduit(Tuple<string, string, string, string, List<string>, List<string>> produit)
        {
            if (produit != null)
            {
                AutoCompleteModele.SelectedItem = produit.Item1;
                AutoCompleteItem.SelectedItem = produit.Item2;
                TextBoxAutre.Text = produit.Item3;
                listBoxAjout.SelectedItem = listBoxAjout.Items.SingleOrDefault(i => i.ToString() == produit.Item4);
                SetInsertionsBC(produit.Item5);
                SetOptionsBC(produit.Item6);
            }
            else
            {
                AutoCompleteModele.SelectedItem = null;
                AutoCompleteItem.SelectedItem = null;
                TextBoxAutre.Text = string.Empty;
                listBoxAjout.SelectedItem = null;
                ReinitBC();
            }
        }

        private List<string> GetInsertionsBC()
        {
            return listBoxInsertions.Items.Cast<string>().OrderBy(i => i).ToList();
        }

        private void SetInsertionsBC(List<string> insertions)
        {
            selectedInsertions.Clear();
            foreach (var item in insertions)
                selectedInsertions.Add(item);
        }

        private List<string> GetOptionsBC()
        {
            return listBoxOptions.Items.Cast<string>().OrderBy(i => i).ToList();
        }

        private void SetOptionsBC(List<string> options)
        {
            selectedOptions.Clear();
            foreach (var item in options)
                selectedOptions.Add(item);
        }

        public Tuple<string, int, decimal> GetProjetProduit()
        {
            return new Tuple<string, int, decimal>(
                    txtDescription.Text, int.Parse(txtQuantite.Text), decimal.Parse(txtPrix.Text)
                );
        }

        public void SetProjetProduit(Tuple<string, int, decimal> projetProduit)
        {
            txtDescription.Text = projetProduit.Item1 ?? string.Empty;
            txtQuantite.Text = projetProduit.Item2.ToString();
            txtPrix.Text = projetProduit.Item3.ToString();
        }

        private void AutoCompleteModele_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckBC();
        }

        private void AutoCompleteModele_TextChanged(object sender, RoutedEventArgs e)
        {
            UpdateAutocompleteTexts(sender);
        }

        private void AutoCompleteItem_TextChanged(object sender, RoutedEventArgs e)
        {
            UpdateAutocompleteTexts(sender);
        }

        private void TextBoxAutre_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAutocompleteTexts(sender);
        }

        private void UpdateAutocompleteTexts(object sender)
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
                                    CheckBC();
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

        private void CheckBC()
        {
            if (AutoCompleteModele.SelectedItem != null)
                if (AutoCompleteModele.SelectedItem.ToString().ToLower().Contains("bc "))
                    SetBCComponents(true);
                else
                    SetBCComponents(false);
            else
                SetBCComponents(false);
        }

        private void SetBCComponents(bool active)
        {
            if (active)
            {
                listBoxAjout.IsEnabled = true;
                listBoxInsertions.IsEnabled = true;
                listBoxOptions.IsEnabled = true;

                AutoCompleteInsertions.IsEnabled = true;
                AutoCompleteOptions.IsEnabled = true;
                
                btnAdd.IsEnabled = true;
                btnRemove.IsEnabled = true;
                btnAddOption.IsEnabled = true;
                btnRemoveOption.IsEnabled = true;
            }
            else
            {
                listBoxAjout.IsEnabled = false;
                listBoxInsertions.IsEnabled = false;
                listBoxOptions.IsEnabled = false;

                AutoCompleteInsertions.IsEnabled = false;
                AutoCompleteOptions.IsEnabled = false;

                btnAdd.IsEnabled = false;
                btnRemove.IsEnabled = false;
                btnAddOption.IsEnabled = false;
                btnRemoveOption.IsEnabled = false;

                ReinitBC();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AutoCompleteInsertions.SelectedItem != null)
            {
                selectedInsertions.Add(AutoCompleteInsertions.SelectedItem.ToString());
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxInsertions.SelectedItem != null)
            {
                selectedInsertions.Remove(listBoxInsertions.SelectedItem.ToString());
            }
        }

        private void btnAddOption_Click(object sender, RoutedEventArgs e)
        {
            if (AutoCompleteOptions.SelectedItem != null)
            {
                selectedOptions.Add(AutoCompleteOptions.SelectedItem.ToString());
            }
        }

        private void btnRemoveOption_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxOptions.SelectedItem != null)
            {
                selectedOptions.Remove(listBoxOptions.SelectedItem.ToString());
            }
        }

        private void txtPrixQte_TextChanged(object sender, TextChangedEventArgs e)
        {
            int qte;
            decimal prix;

            if (int.TryParse(txtQuantite.Text, out qte) && decimal.TryParse(txtPrix.Text.Replace(" ", string.Empty), style, cultureInfo, out prix) && 
                qte >= 0 && qte < 100)
            {
                txtTotal.Foreground = BLACK;
                txtTotal.Text = string.Empty + (qte * prix);
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

        public bool Validate()
        {
            if (AutoCompleteModele.SelectedItem == null && AutoCompleteItem.SelectedItem == null && TextBoxAutre.Text.Equals(string.Empty))
            {
                AutoCompleteModele.BorderBrush = RED;
                return false;
            }

            if (txtTotal.Foreground == RED)
            {
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

        public void ResetColors()
        {
            AutoCompleteModele.BorderBrush = BORDER;
        }

        private void AutoCompleteModele_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckBC();
        }
    }
}
