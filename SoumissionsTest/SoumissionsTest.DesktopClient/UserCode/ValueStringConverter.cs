using System;
using System.Windows.Data;

namespace LightSwitchApplication.UserCode
{
    public class ScreenConverters
    {
        public class ValueStringConverter : IValueConverter
        {
            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var textResult = "";
                if ((int)value == 0)
                {
                    textResult = "0$";
                }
                else
                {
                    textResult = string.Format("{0:#,###.00$}", decimal.Round((int)value, 2));
                }

                return textResult;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
