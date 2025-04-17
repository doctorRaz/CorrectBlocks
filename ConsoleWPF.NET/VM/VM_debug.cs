using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drzTools.VievModel
{
    public partial class VM : INotifyPropertyChanged
    {
         public string IsVisibilityNumderGrid =>  _VisibilityCollapse;




#if DEBUG
        private readonly string _VisibilityCollapse = "Visible";
#else
        readonly string _VisibilityCollapse = "Collapsed";
#endif


    }
}
