using System;
using System.Globalization;
using System.Text;

using DemoBoundRadioButtons;

using drzTools.VievModel;
using drzTools.WPF;

namespace drzTools.CMD
{
    [Flags]
    public enum S
    {

        S1 = 1 << 1,
        S2 = 1 << 2,
        S3 = 1 << 3,
    }

    [Flags]
    public enum T
    {
        T1 = 1 << 4,
        T2 = 1 << 5,
        T3 = 1 << 6,
    }

    public enum ST
    {
        S1 = 1 << 1,
        S2 = 1 << 2,
        S3 = 1 << 3,
        T1 = 1 << 4,
        T2 = 1 << 5,
        T3 = 1 << 6,
    }

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
#if NET
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            S s = new S();
            T t = new T();

            s = S.S1 | S.S2;
            t = T.T1 | T.T2;

            var i = (int)s + (int)t;

            ST st = (ST)i;
            bool bs1 = (bool)st.HasFlag(ST.S1);
            bs1 = (bool)st.HasFlag(ST.S2);
            bs1 = (bool)st.HasFlag(ST.S3);
            bs1 = (bool)st.HasFlag(ST.T1);
            bs1 = (bool)st.HasFlag(ST.T2); 
            bs1 = (bool)st.HasFlag(ST.T3);

            //BlockNormalizeSettingsEnum en=new BlockNormalizeSettingsEnum();
            //en = BlockNormalizeSettingsEnum.SetLayer0 |BlockNormalizeSettingsEnum.SetByLayer;
            //bool bb = (bool)en.HasFlag(BlockNormalizeSettingsEnum.SetLayer0);



            MainPage mp = new MainPage();

            mp.ShowDialog();

            Config cfg = new Config(); //читаю конфигурацию
            var vm = cfg.vm;

            //string product = "drzBLFIX";
            //string version = "0.0.1";
            //vm.Title = $"{product} v {version}";

            GuiTools win = new GuiTools(vm);

            bool? isOk = win.ShowDialog();

            cfg.Serialize();

            if (isOk == true)
            {
                //чето делаем
            }
            else
            {
                return;
            }
            //Console.WriteLine("Hello");
            //Console.ReadKey();
        }
    }



}
