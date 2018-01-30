using System.Windows.Controls;
using System.Windows.Media;

namespace SilverlightCustomControls
{
    public partial class ProdIndicator : UserControl
    {
        private static SolidColorBrush onColor = new SolidColorBrush(Color.FromArgb(255, 0, 222, 111));
        private static SolidColorBrush offColor = new SolidColorBrush(Color.FromArgb(255, 255, 104, 104));

        public ProdIndicator()
        {
            InitializeComponent();
        }

        public void SetColor(bool on)
        {
            circle.Fill = on ? onColor : offColor;
        }
    }
}
