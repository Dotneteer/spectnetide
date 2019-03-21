using System;
using System.Globalization;
using System.Windows.Data;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class converts integer and short values to 4 digit hexadecimal strings
    /// </summary>
    public class Hex4Converter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort usValue) return $"{usValue:X4}";
            if (value is int iValue) return $"{iValue:X4}";
            if (value is short sValue) return $"{sValue:X4}";
            if (value is uint uiValue) return $"{uiValue:X4}";
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}