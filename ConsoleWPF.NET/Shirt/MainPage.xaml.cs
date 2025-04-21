using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
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
        private EntityDrawOrder _size; 
        private EntityProperties _size2;

        public Shirt()
        {
            Size = EntityDrawOrder.Back;
        }

        public EntityProperties Size2
        {
            get { return _size2; }
            set
            {
                _size2 = value;
                OnPropertyChanged();
            }
        }
        public EntityDrawOrder Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }



        /// <summary>Оповещатель</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Оповещатель</summary>
        /// <param name="prop">Имя свойства вызвавшего событие</param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                                new PropertyChangedEventArgs(prop));
        }


     
    }

    public enum EntityDrawOrder
    {
        /// <summary>
        /// Не менять порядок следования
        /// </summary>
        DoNotChange,
        /// <summary>
        /// Передвинуть вперед всех остальных примитивов
        /// </summary>
        Forward,
        /// <summary>
        /// Передвинуть позади всех остальных примитивов
        /// </summary>
        Back,
    }

    public enum EntityProperties
    {
        /// <summary> Свойства не менять </summary>
        DoNotChange,
        /// <summary>
        /// Установить "По блоку"
        /// </summary>
        ByBlock,
        /// <summary>
        /// Установить "По слою"
        /// </summary>
        ByLayer,
    }
    public enum Sizes
    {
        Small,
        Medium,
        Large
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

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string paramString = parameter.ToString();// as string;
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
            string paramString = parameter.ToString();// as string;
            if (paramString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return Enum.Parse(targetType, paramString);
        }
    }



    public class EnumToBooleanConverter0 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string paramString = parameter as string;
            //if (paramString == null)
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            //if (Enum.IsDefined(value.GetType(), value) == false)
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            //object paramValue = Enum.Parse(value.GetType(), paramString);
            //return paramValue.Equals(value);
            return value ?. Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string paramString = parameter as string;
            //if (paramString == null)
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            //return Enum.Parse(targetType, paramString);
            return value?.Equals(true)==true ?parameter:Binding.DoNothing;
        }
    }
}