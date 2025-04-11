using System;

using drzTools.WPF;

namespace drzTools.CMD
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GuiTools win=new GuiTools();
            win.ShowDialog();

            //Console.WriteLine("Hello");
            //Console.ReadKey();
        }
    }
}
