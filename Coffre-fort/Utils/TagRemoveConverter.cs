using System;
using System.Globalization;
using System.Windows.Data;

namespace Coffre_fort.Utils
{
    public class TagRemoveConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = tag string ; values[1] = PasswordEntry
            if (values[0] is string tag && values[1] is PasswordEntry entry)
            {
                return Tuple.Create(entry, tag);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
