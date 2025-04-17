using System;
using System.Globalization;
using System.Text;

using drzTools.VievModel;
using drzTools.WPF;

namespace drzTools.CMD
{

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
#if NET
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            BlockNormalizeSettingsEnum en=new BlockNormalizeSettingsEnum();
            en = BlockNormalizeSettingsEnum.SetLayer0 |BlockNormalizeSettingsEnum.SetByLayer;
            bool bb = (bool)en.HasFlag(BlockNormalizeSettingsEnum.SetLayer0);
            VM vm = new VM();

            string product = "drzBLFIX";
            string version = "0.0.1";
            vm.Title = $"{product} v {version}";

            GuiTools win =new GuiTools(vm);
            bool? isOk = win.ShowDialog();

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


    public enum BlockNormalizeSettingsEnum
    {
        /// <summary>умолчание </summary>
        Default = 0,
        //тип Entity если оба нули, значит не менять
        /// <summary>тип Entity по слою </summary>
        SetByLayer = 1 << 1,
        /// <summary> тип Entity по блоку</summary>
        SetByBlock = 1 << 2,
        //цвет если оба нули, значит не менять
        /// <summary> цвет Entity по слою</summary>
        ColorByLayer = 1 << 3,
        /// <summary> цвет Entity по блоку</summary>
        ColorByBlock = 1 << 4,
        // вес Entity если оба нули, значит не менять
        /// <summary> вес Entity по слою</summary>
        LineweightByLayer = 1 << 5,
        /// <summary> вес Entity по блоку</summary>
        LineweightByBlock = 1 << 6,
        /// <summary>trye-Entity на слой zero<br>false-Entity слой не менять</br></summary>
        SetLayer0 = 1 << 7,
        /// <summary>trye-топить маскировку<br>false-не топить маскировку</br>  </summary>
        SetWipeoutBack = 1 << 8,
        /// <summary>trye-топить штриховку<br>false-не топить маскировку</br>  </summary>
        SetHatchtBack = 1 << 9,
        //одинаковые масштабы Block если оба нули, значит не менять
        /// <summary>одинаковые масштабы Block On</summary>
        EqualScaleOn = 1 << 10,
        /// <summary>одинаковые масштабы Block Off</summary>
        EqualScaleOff = 1 << 11,
        //Разрешить расчленение Block если оба нули, значит не менять
        /// <summary>Explodable Block On</summary>
        SetBlockExplodeable = 1 << 12,
        /// <summary>Explodable Block Off</summary>
        SetBlockUnexplodeable = 1 << 13
    }
}
