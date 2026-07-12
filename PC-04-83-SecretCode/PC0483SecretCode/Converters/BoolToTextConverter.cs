using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PC0483SecretCode.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool and true ? "Text To Code" : "Code To Text";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}