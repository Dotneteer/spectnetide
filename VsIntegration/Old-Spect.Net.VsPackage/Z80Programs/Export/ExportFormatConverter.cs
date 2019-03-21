using System;
using System.Globalization;
using System.Windows.Data;

namespace Spect.Net.VsPackage.Z80Programs.Export
{
    /// <summary>
    /// Converts an ExportFormat value to checkbox state
    /// </summary>
    public class ExportFormatConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ExportFormat formatValue
                && Enum.TryParse<ExportFormat>(parameter?.ToString() ?? "", out var format))
            {
                return formatValue == format;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue
                && Enum.TryParse<ExportFormat>(parameter?.ToString() ?? "", out var format))
            {
                return boolValue ? format : 0;
            }
            return 0;
        }
    }
}