using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using drzTools.Enums;

namespace drzTools.ViewModel
{
    public class RadioButtonConverterBy : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return (value.ToString() == parameter.ToString());
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return (bool)value ? Enum.Parse(typeof(ByAll), parameter.ToString(), true) : null;
        //}
    }


}
