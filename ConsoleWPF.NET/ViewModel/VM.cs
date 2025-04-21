using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using drzTools.Enums;

namespace drzTools.ViewModel
{
    [Serializable]
    public partial class VM : INotifyPropertyChanged
    {
        /// <summary>
        /// титул окна
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title => DataSetWpfOpt.AppProductName + " " + DataSetWpfOpt.Version;




        public ByAll Layer { get; set; }




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
