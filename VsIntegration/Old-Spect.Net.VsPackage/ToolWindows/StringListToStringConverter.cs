using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class converts a list of strings into a comma separated string
    /// </summary>
    public class StringListToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is List<string> stringList 
                ? string.Join(", ", stringList) 
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}