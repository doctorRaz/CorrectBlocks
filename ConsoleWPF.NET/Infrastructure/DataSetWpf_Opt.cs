/*
*        Copyright doctorRAZ 2014-2025 by Разыграев Андрей
*
*        Licensed under the Apache License, Version 2.0 (the "License");
*        you may not use this file except in compliance with the License.
*        You may obtain a copy of the License at
*
*            http://www.apache.org/licenses/LICENSE-2.0
*
*        Unless required by applicable law or agreed to in writing, software
*        distributed under the License is distributed on an "AS IS" BASIS,
*        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*        See the License for the specific language governing permissions and
*        limitations under the License.
*/


using System;
using System.IO;
using System.Reflection;


namespace drzTools
{
    /// <summary> Пути разделители и пр.</summary>
    public class DataSetWpfOpt
    {
        #region Служебные
        /// <summary>Домен машины</summary>
        internal static string Userdomain = System.Environment.GetEnvironmentVariable("USERDOMAIN");

        #endregion
        #region ВЕРСИЯ ПРОГРАММЫ
        /// <summary>Версия программы</summary>
        internal static System.Version SysVersion => Asm.GetName().Version;

        /// <summary>Версия мажор</summary>
        internal static int Major => SysVersion.Major;

        /// <summary>Версия минор</summary>
        internal static int Minor => SysVersion.Minor;

        /// <summary>Версия билд</summary>
        public static int Build => SysVersion.Build;

        /// <summary>Версия ревизия</summary>
        internal static int Revision => SysVersion.Revision;

        /// <summary>Бета или нет, чет нечет</summary>
        //static string _sBeta => iMinor == 0 || iMinor >3 ? "" : iMinor == 1 ? "<alfa>" : "<beta>";
        static string _sBeta => Minor > 0 & Minor <= 5 ? "<beta>" : "";
        #endregion

        #region Assembly

        /// <summary>Сборка содержащая текущий исполняемый код</summary>
        public static Assembly Asm => Assembly.GetExecutingAssembly();

        /// <summary> Титул программы </summary>
        public static string TitleAttribute => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyTitleAttribute),
            false) as AssemblyTitleAttribute).Title;

        /// <summary>Описание программы </summary>
        public static string Description => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyDescriptionAttribute),
            false) as AssemblyDescriptionAttribute).Description;//!описание

        /// <summary>Конфигурация программы </summary>
        public static string Configuration => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyConfigurationAttribute),
            false) as AssemblyConfigurationAttribute).Configuration;

        /// <summary>Компания </summary>
        public static string Company => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyCompanyAttribute),
            false) as AssemblyCompanyAttribute).Company;

        /// <summary>Продукт </summary>
        public static string Product => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyProductAttribute),
            false) as AssemblyProductAttribute).Product;

        /// <summary>копирайт</summary>
        public static string Copyright => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyCopyrightAttribute),
            false) as AssemblyCopyrightAttribute).Copyright;

        /// <summary>торговая марка</summary>
        public static string Trademark => (Attribute.GetCustomAttribute(
            Asm,
            typeof(AssemblyTrademarkAttribute),
            false) as AssemblyTrademarkAttribute).Trademark;

        /// <summary>ProductVersion - Версия программы
        /// <br>для идентификации</br>
        /// </summary>
        public static string InformationalVersionAttribut => (Attribute.GetCustomAttribute(
                   Asm,
                   typeof(AssemblyInformationalVersionAttribute),
                   false) as AssemblyInformationalVersionAttribute).InformationalVersion;

        /// <summary>Полный путь к сборке </summary>
        public static string AsmFulPath => Asm.Location;

        /// <summary>Имя сборки без расширения</summary>
        public static string AsmFileNameWithoutExtension => Path.GetFileNameWithoutExtension(AsmFulPath);

        /// <summary>Имя сборки с расширением</summary>
        public static string AsmFileName => Path.GetFileName(AsmFulPath);

        /// <summary>версия программы </summary>
        public static string Version => Major.ToString()
                                         + "."
                                         + Minor.ToString()
                                         + "."
                                         + Build.ToString()
                                         + _sBeta//_sysVersion.ToString() + _sBeta;
                                         ;
        /// <summary>Полная версия с окончанием </summary>
        public static string VersionFull => Major.ToString()
                                             + "."
                                             + Minor.ToString()
                                             + "."
                                             + Build.ToString()
                                             + "."
                                             + Revision.ToString()
                                             + _sBeta;//_sysVersion.ToString() + _sBeta;

        /// <summary>Полная версия без окончания </summary>
        public static string VersionShort => Major.ToString()
                                             + "."
                                             + Minor.ToString()
                                             + "."
                                             + Build.ToString()
                                             + "."
                                             + Revision.ToString();

        /// <summary>Дата сборки</summary>
        public static string DateRelis()
        {
            DateTime result = new DateTime(2000, 1, 1);
            result = result.AddDays(Build);
            result = result.AddSeconds(Revision * 2);

#if DEBUG
            return result.ToString();
#else
            return result.ToLongDateString();
#endif
        }


        #endregion

        #region О программе

        /// <summary>Название программы </summary>
        internal static string AppProductName => System.Windows.Forms.Application.ProductName;

        /// <summary>Мажор версия программы </summary>

        internal static string AppMajor => ((System.Windows.Forms.Application.ProductVersion).Split('.'))[0];


        #endregion

        #region  ПУТИ

        /// <summary>Путь к Program DATA</summary>
        private static string _sProgramData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        /// <summary>Общий путь к пользовательским настройкам 
        /// <br>c:\Users\User\AppData\Roaming\ </br></summary>
        private static string _sUserAppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>Путь к рабочему столу</summary>
        public static string UserDesktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>Имя машины</summary>
        public static string MachineName => Environment.MachineName;

        //public static string sTemp=>Path.GetTempPath();


        #endregion

    }
    //***B

    #region ДЛЯ АК не начинал
#if AC && NC
    /// <summary> Формируем PS только для АК непилено совсем!!!!!!  </summary>
    public class FieldPageSets
    {
        public FieldPageSets(FieldsFrmt frameFrmt, Transaction Tx, Database db, int conter)
        {
            //TODO: потом брать из настроек ИНИ
string sPlot ="DWG To PDF.pc3";
            string sStyle = "monochrome.ctb";//заглушка стиль


            App.Document doc = App.Application.DocumentManager.MdiActiveDocument;
            nanoCAD.Document comDoc = doc.AcadDocument as nanoCAD.Document;
            nanoCAD.InanoCADPlot plot = (nanoCAD.InanoCADPlot)comDoc.Plot;
            OdaX.AcadLayout activeLayout =comDoc.ActiveLayout;
            nanoCAD.InanoCADPlotCustomParams param = plot.CustomPlotSettings[activeLayout];
            param.Multipage = true;
            param.PlotAreas.Clear();
            plot.CustomPlotSettings[activeLayout] = param;

            Layout layout = Tx.GetObject(frameFrmt.layoutId, OpenMode.ForWrite) as Layout;

        
            PlotSettings ps = new PlotSettings(layout.ModelType);
            ps.CopyFrom(layout);
            PlotSettingsValidator psv = PlotSettingsValidator.Current;
            psv.SetPlotConfigurationName(ps, sPlot, null);
            psv.RefreshLists(ps);
            psv.SetPlotPaperUnits(ps, PlotPaperUnit.Millimeters);
            psv.SetPlotRotation(ps, PlotRotation.Degrees000);
            psv.SetPlotConfigurationName(ps, sPlot, "ISO_full_bleed_A1_(841.00_x_594.00_MM)");
            psv.SetCurrentStyleSheet(ps, sStyle);
            psv.SetPlotWindowArea(ps, frameFrmt.points);
            psv.SetPlotType(ps, Tds.PlotType.Window);
            psv.SetUseStandardScale(ps, false);
            psv.SetCustomPrintScale(ps, new CustomScale(1, frameFrmt.dScale));//масштаб
            ps.PrintLineweights = true;

            psv.SetPlotCentered(ps, true);

            ps.PlotSettingsName = String.Format("{0}{1}", conter, "PS");

            ps.AddToPlotSettingsDictionary(db);
            Tx.AddNewlyCreatedDBObject(ps, true);

            psv.RefreshLists(ps);
            layout.CopyFrom(ps);
        }
    }
#endif
    #endregion

}