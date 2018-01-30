using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch;
using System.Linq;
using System.Windows.Controls;
using SilverlightCustomControls;
using System.Windows;

namespace LightSwitchApplication
{
    public partial class SuiviProduits
    {
        partial void SuiviProduits_Activated()
        {
            this.FindControl("gridProduits").ControlAvailable += (obj, ev) => 
            {
                DataGrid grid = ev.Control as DataGrid;
                grid.RowEditEnded += (obj2, ev2) => 
                {
                    DataGridRow row = ev2.Row;
                    UpdateRow(grid, row);
                };

                grid.LoadingRow += (obj2, ev2) =>
                {
                    DataGridRow row = ev2.Row;

                    row.Loaded += (obj3, ev3) =>
                    {
                        UpdateRow(grid, row);
                    };
                };
            };
        }


        private void UpdateRow(DataGrid grid, DataGridRow row)
        {
            ProjetProduit pp = row.DataContext as ProjetProduit;
            bool isProd = pp.E_Accept && pp.E_Approb && pp.E_D100 && pp.E_D85 && pp.E_Diagramme && pp.E_Implem && pp.E_Kickoff && pp.E_Refreg && pp.E_SeqCtrl;
            
            ProdIndicator indicator = (grid.Columns[1].GetCellContent(row) as UIElement)
                .GetChildren().First()
                .GetChildren().First()
                .GetChildren().First()
                .GetChildren().First()
                .GetChildren().First()
                .GetChildren().First()
                .GetChildren().First() as ProdIndicator;

            indicator.SetColor(isProd);
        }

    }
}