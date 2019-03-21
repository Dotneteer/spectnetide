using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This converter creates a visibility value from an bool.
    /// </summary>
    public class BoolToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Hidden : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}