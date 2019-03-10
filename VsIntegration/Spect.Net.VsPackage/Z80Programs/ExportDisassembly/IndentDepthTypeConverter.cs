using System;
using System.Globalization;
using System.Windows.Data;

namespace Spect.Net.VsPackage.Z80Programs.ExportDisassembly
{
    /// <summary>
    /// Converts an IndentDepthType value to checkbox state
    /// </summary>
    public class IndentDepthTypeConverter: IValueConverter  
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IndentDepthType formatValue
                && Enum.TryParse<IndentDepthType>(parameter?.ToString() ?? "", out var format))
            {
                return formatValue == format;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue
                && Enum.TryParse<IndentDepthType>(parameter?.ToString() ?? "", out var format))
            {
                return boolValue ? format : 0;
            }
            return 0;
        }
    }
}