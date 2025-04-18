using System;
using System.Globalization;
using System.Windows.Data;

using System.ComponentModel;
 
using System.Windows;
 

using DemoBoundRadioButtons;

namespace drzTools
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramString = parameter as string;
            if (paramString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            object paramValue = Enum.Parse(value.GetType(), paramString);
            return paramValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramString = parameter as string;
            if (paramString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return Enum.Parse(targetType, paramString);
        }
    }

}
