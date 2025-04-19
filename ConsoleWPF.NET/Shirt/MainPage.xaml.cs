using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DemoBoundRadioButtons
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new Shirt();
            //Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }

    public class Shirt : INotifyPropertyChanged
    {
        private Sizes _size;

        public Shirt()
        {
            Size = Sizes.Small;
        }

        public Sizes Size
        {
            get { return _size; }
            set
            {
                _size = _size | value;
                RaisePropertyChanged("Size");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [Flags]
    public enum Sizes
    {
        Small = 1 << 1,
        Medium = 1 << 2,
        Large = 1 << 3,
        Test1 = 1 << 4,
        Test2 = 1 << 5,
        Test3 = 1 << 6,
    }

    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value.ToString() == parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(typeof(Sizes), parameter.ToString(), true) : null;
        }
    }
    public class RadioButtonConverter0 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Enum)value).HasFlag((Enum)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    //https://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
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