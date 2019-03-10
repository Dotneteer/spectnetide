using System;
using System.Globalization;
using System.Windows.Data;

namespace Spect.Net.VsPackage.Z80Programs.ExportDisassembly
{
    /// <summary>
    /// Converts an LineLengthType value to checkbox state
    /// </summary>
    public class LineLengthTypeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LineLengthType formatValue
                && Enum.TryParse<LineLengthType>(parameter?.ToString() ?? "", out var format))
            {
                return formatValue == format;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue
                && Enum.TryParse<LineLengthType>(parameter?.ToString() ?? "", out var format))
            {
                return boolValue ? format : 0;
            }
            return 0;
        }

    }
}