using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Globalization;
using System.Threading;

namespace SilverlightCustomControls
{

    public class AutoCompleteConverter: IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value == "")
            {
                return null;
            }
            else
            {
                if (((string)value).Contains("("))
                {
                    return value;
                } else
                {
                    return "-" + value;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TextBoxConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CurrencyConverter : IValueConverter
    {
        #region IValueConverter Members

        CultureInfo cultureInfo;
        NumberStyles style;

        public CurrencyConverter()
        {
            cultureInfo = Thread.CurrentThread.CurrentCulture;

            style = (NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol) ^ NumberStyles.AllowThousands;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                decimal returnValue;
                decimal.TryParse(value.ToString(), style, cultureInfo, out returnValue);
                return decimal.Round(returnValue, 2);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal returnValue;
            decimal.TryParse(value.ToString(), style, cultureInfo, out returnValue);
            return decimal.Round(returnValue, 2);
        }

        #endregion
    }

    public class ItemsConverter : IValueConverter
    {
        #region IValueConverter Members

        public ItemsConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? string.Empty : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
