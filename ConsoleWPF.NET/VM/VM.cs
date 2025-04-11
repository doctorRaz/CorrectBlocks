using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace drzTools.VM
{
    public class VM : INotifyPropertyChanged
    {











        #region PROPERTYCHANGED

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
        #endregion
    }
}
