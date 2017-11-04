﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.Wpf.SpectrumControl
{
    public class VmBorderConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is VmState)) return value;
            string styleName;
            switch ((VmState)value)
            {
                case VmState.Stopped:
                    styleName = "BVmStopped";
                    break;
                case VmState.Paused:
                    styleName = "BVmPaused";
                    break;
                default:
                    styleName = "BVmRunning";
                    break;
            }
            return (SolidColorBrush)Application.Current.Resources[styleName];
        }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}