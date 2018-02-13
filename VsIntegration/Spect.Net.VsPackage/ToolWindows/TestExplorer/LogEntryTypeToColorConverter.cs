using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// Converts test state value to a fill color
    /// </summary>
    public class LogEntryTypeToColorConverter: IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogEntryType type)
            {
                switch (type)
                {
                    case LogEntryType.Success:
                        return new SolidColorBrush(Colors.Green);
                    case LogEntryType.Fail:
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Color.FromRgb(0x00, 0xAF, 0xFF));
                }
            }
            return value;
        }

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}