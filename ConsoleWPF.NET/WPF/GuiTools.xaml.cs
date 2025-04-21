using System.Windows;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;

 
using drzTools.ViewModel;

namespace drzTools.WPF
{
    /// <summary>
    /// Логика взаимодействия для GuiTools.xaml
    /// </summary>
    public partial class GuiTools : Window
    {
        /// <summary>
        /// Gets or sets the viev model.
        /// </summary>
        /// <value>
        /// The vm.
        /// </value>
        VM vm { get; set; }
        public GuiTools(VM _vm)
        {
            DataContext = _vm;
            InitializeComponent();
        }
    }
}
