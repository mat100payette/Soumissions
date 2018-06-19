using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Printing;

namespace SilverlightCustomControls
{
    public partial class FeuilleCommande : UserControl
    {
        private const double OPACITY_FULL = 1;
        private const double OPACITY_TRANSPARENT = 0.2;

        private PrintDocument withEventsField_pd;
        private int numProjet = 0;
        private List<Tuple<int, string, string, decimal, decimal>> projetProduits;
        private int rowCount = 0;
        private double totalRowsHeight = 0;
        private SolidColorBrush White;
        private SolidColorBrush Black;
        private PagedCollectionView itemListView;
        private int pageCount = 0;
        private int pageIndex = 0;

        public FeuilleCommande()
        {
            // THIS SETS THE FRAMEWORK ELEMENT'S CULTURE TO THE CURRENT CULTURE !! //
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

            White = new SolidColorBrush(Colors.White);
            Black = new SolidColorBrush(Colors.Black);

            InitializeComponent();

            GridModeles.LoadingRow += GridModeles_LoadingRow;
            GridModeles.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            pd = new PrintDocument();
            
        }

        private void GridModeles_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.SizeChanged -= Row_SizeChanged;
            e.Row.SizeChanged += Row_SizeChanged;

            rowCount = 0;
            totalRowsHeight = 0;
        }

        private void Row_SizeChanged(object sender, SizeChangedEventArgs e)
        {     
            if (totalRowsHeight + e.NewSize.Height > GridModeles.Height - GridModeles.ColumnHeaderHeight)
            {
                pagerProduits.PageSize = rowCount;
            }

            rowCount++;
            totalRowsHeight += e.NewSize.Height;
        }

        private PrintDocument pd
        {
            get { return withEventsField_pd; }
            set
            {
                if (withEventsField_pd != null)
                {
                    withEventsField_pd.BeginPrint -= pd_BeginPrint;
                    withEventsField_pd.PrintPage -= pd_PrintPage;
                    withEventsField_pd.EndPrint -= pd_EndPrint;
                }
                withEventsField_pd = value;
                if (withEventsField_pd != null)
                {
                    withEventsField_pd.BeginPrint += pd_BeginPrint;
                    withEventsField_pd.PrintPage += pd_PrintPage;
                    withEventsField_pd.EndPrint += pd_EndPrint;
                }
            }
        }

        private void pd_BeginPrint(object sender, BeginPrintEventArgs e)
        {
            GridModeles.SelectedIndex = -1;
            if (pagerProduits.PageIndex != 0) {
                pagerProduits.PageIndex = pagerProduits.PageSize == 0 ? -1 : 0;
            }
            isBousquet.Visibility = Visibility.Collapsed;
            isBousquet.IsEnabled = false;
            txtBousquet.Visibility = Visibility.Collapsed;
            txtNagas.Visibility = Visibility.Collapsed;
            button.IsEnabled = false;
            button.Opacity = 0;

            pageCount = pagerProduits.PageCount;
            pageIndex = 0;
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            pagerProduits.PageIndex = pagerProduits.PageSize == 0 ? -1 : pageIndex;
            pagerProduits.UpdateLayout();
            e.PageVisual = LayoutRoot;
            e.HasMorePages = pageIndex < pageCount - 1;
            pageIndex++;
        }

        private void pd_EndPrint(object sender, EndPrintEventArgs e)
        {
            isBousquet.Visibility = Visibility.Visible;
            isBousquet.IsEnabled = true;
            txtBousquet.Visibility = Visibility.Visible;
            txtNagas.Visibility = Visibility.Visible;
            button.IsEnabled = true;
            button.Opacity = 255;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            pd.Print(string.Format("Commande {0}", numProjet));
        }

        public void SetNumProjet(int num)
        {
            numProjet = num;
        }

        public void SetProjetProduits(List<Tuple<int, string, string, decimal, decimal>> projetProduits)
        {
            this.projetProduits = projetProduits;
            itemListView = new PagedCollectionView(this.projetProduits);
            pagerProduits.Source = itemListView;
            GridModeles.ItemsSource = itemListView;
        }

        private void isBousquet_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = (sender as CheckBox);

            SetCheckBoxState(box);
        }

        private void isBousquet_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox box = (sender as CheckBox);

            if (!box.IsChecked.HasValue)
            {
                box.SetValue(ToggleButton.IsCheckedProperty, true);
            }

            SetCheckBoxState(box);
        }

        private void SetCheckBoxState(CheckBox box)
        {
            if (box != null)
            {
                if (box.IsChecked.HasValue)
                {
                    if (box.IsChecked.Value)
                    {
                        txtBousquet.Opacity = OPACITY_FULL;
                        txtNagas.Opacity = OPACITY_TRANSPARENT;
                        bousquet.Visibility = Visibility.Visible;
                        nagas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtBousquet.Opacity = OPACITY_TRANSPARENT;
                        txtNagas.Opacity = OPACITY_FULL;
                        bousquet.Visibility = Visibility.Collapsed;
                        nagas.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public void SetVenduASource(List<string> list, string selected)
        {
            comboCie.ItemsSource = list;
            comboCie.SelectedItem = selected;
        }

        public void SetVenduSelectionChanged(SelectionChangedEventHandler handler)
        {
            comboCie.SelectionChanged -= handler;
            comboCie.SelectionChanged += handler;
        }
    }
}
