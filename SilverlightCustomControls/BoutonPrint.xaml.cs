using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Printing;

namespace SilverlightCustomControls
{
    public partial class BoutonPrint : UserControl
    {
        private PrintDocument withEventsField_pd;
        private PrintPageEventArgs printArgs;
        private UIElement screen;


        public BoutonPrint()
        {
            InitializeComponent();
            printArgs = new PrintPageEventArgs();
        }

        private PrintDocument pd
        {
            get { return withEventsField_pd; }
            set
            {
                if (withEventsField_pd != null)
                {
                    withEventsField_pd.PrintPage -= pd_PrintPage;
                    withEventsField_pd.EndPrint += pd_EndPrint;
                }
                withEventsField_pd = value;
                if (withEventsField_pd != null)
                {
                    withEventsField_pd.PrintPage += pd_PrintPage;
                    withEventsField_pd.EndPrint += pd_EndPrint;
                }
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = printArgs.PageVisual;

            double scaleX = 1;
            double scaleY = 1;
            
            var screenElement = (FrameworkElement)screen;

            if (e.PrintableArea.Height < screenElement.ActualHeight)
            {
                scaleY = e.PrintableArea.Height / screenElement.ActualHeight;
            }

            if (e.PrintableArea.Width < screenElement.ActualWidth && e.PrintableArea.Width / screenElement.ActualWidth < scaleX)
            {
                scaleX = e.PrintableArea.Width / screenElement.ActualWidth;
            }

            if (scaleX < 1)
            {
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = scaleX;
                scaleTransform.ScaleY = scaleY;
                
                screen.RenderTransform = scaleTransform;
            }

            button.IsEnabled = false;
            button.Opacity = 0;
        }

        private void pd_EndPrint(object sender, EndPrintEventArgs e)
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform() { ScaleX = 1 , ScaleY = 1 });
            screen.RenderTransform = transformGroup;

            button.IsEnabled = true;
            button.Opacity = 255;
        }

        public void print()
        {
            pd = new PrintDocument();
            withEventsField_pd.Print(string.Format("Rapport"));
        }

        public void addClickHandler(RoutedEventHandler handler)
        {
            button.Click -= handler;
            button.Click += handler;
        }

        public void setPrintArg(UIElement element)
        {
            screen = element;
            printArgs.PageVisual = screen;
        }
    }
}
