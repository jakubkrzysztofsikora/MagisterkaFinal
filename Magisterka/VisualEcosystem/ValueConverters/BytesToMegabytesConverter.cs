using System;
using System.Globalization;
using System.Windows.Data;
using Magisterka.Domain.Converters;

namespace Magisterka.VisualEcosystem.ValueConverters
{
    public class BytesToMegabytesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((long) value).ToMegaBytes();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToBytes();
        }
    }
}
