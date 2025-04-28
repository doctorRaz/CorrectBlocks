// See https://aka.ms/new-console-template for more information


using drz.CorrectBlocks.Core;

namespace drzTools.CMD
{


    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine(AppSetttings.ConfigPath);
            AppSetttings.Settings.ApplicationName = "SomeTestApplication";
            AppSetttings.SaveSettings();






            var set = new AppSetttings();

            var path = AppSetttings.Settings;
            AppSetttings.SaveSettings();


        }
    }
}