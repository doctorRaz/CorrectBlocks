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
        private Sizes _size; 
        private Sizes _size2;

        public Shirt()
        {
            Size = Sizes.Large;
        }

        public Sizes Size2
        {
            get { return _size2; }
            set
            {
                _size2 = value;
                OnPropertyChanged();
            }
        }
        public Sizes Size
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


        //    public event PropertyChangedEventHandler PropertyChanged;
        //    public void RaisePropertyChanged(string propertyName)
        //    {
        //        if (PropertyChanged != null)
        //        {
        //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //        }
        //    }
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
}