using System;
using System.Windows;
using System.Windows.Data;

namespace CelltechFinal.Converters
{
    public class MetodoPagoVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string metodoPago)
            {
                return metodoPago == "Efectivo" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}