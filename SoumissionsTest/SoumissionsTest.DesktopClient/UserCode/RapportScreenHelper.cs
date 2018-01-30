using C1.Silverlight.Chart;
using Microsoft.LightSwitch.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LightSwitchApplication.UserCode
{
    public static class RapportScreenHelper
    {
        public static void SetChartMin(this IScreenObject screen, double min)
        {
            screen.FindControl("Tendance").ControlAvailable += ((o, e) =>
            {
                C1Chart graphique = e.Control as C1Chart;

                graphique.BeginUpdate();

                graphique.View.AxisX.Min = min;

                graphique.EndUpdate();
            });
        }

        public static void SetChartMax(this IScreenObject screen, double max)
        {
            screen.FindControl("Tendance").ControlAvailable += ((o, e) =>
            {
                C1Chart graphique = e.Control as C1Chart;

                graphique.BeginUpdate();

                graphique.View.AxisX.Max = max;

                graphique.EndUpdate();
            });
        }

        public static void SetControlRatioValue(this IScreenObject screen, string controlName, string text, decimal valueA, decimal valueB)
        {
            var textResult = "";
            if (valueB == 0)
            {
                textResult = text + double.PositiveInfinity;
            }
            else
            {
                textResult = text + decimal.Round(valueA / valueB, 3);
            }

            IContentItemProxy control = screen.FindControl(controlName);

            control.SetProperty("Text", textResult);
        }

        public static void setControlMoneyValue(this IScreenObject screen, string controlName, string text, decimal value)
        {
            var textResult = "";
            if (value == 0)
            {
                textResult = text + "0$";
            }
            else
            {
                textResult = text + string.Format("{0:#,###.00$}", decimal.Round(value, 2));
            }

            IContentItemProxy control = screen.FindControl(controlName);

            control.SetProperty("Text", textResult);
        }

        public static void InitColorRectangle(this Rectangle rect, Color col)
        {
            rect.Width = 10;
            rect.Height = 10;
            rect.RadiusX = 2;
            rect.RadiusY = 2;
            rect.Stroke = new SolidColorBrush(Colors.DarkGray);
            rect.Fill = new SolidColorBrush(col);
        }
    }
}
