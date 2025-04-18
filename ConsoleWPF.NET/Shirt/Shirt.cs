using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drzTools.Silverlight
{
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
                _size = value;
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

    public enum Sizes
    {
        Small,
        Medium,
        Large
    }

}
