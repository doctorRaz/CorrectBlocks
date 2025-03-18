

using System;
using System.IO;
using System.Reflection;


#if NC
using Teigha.DatabaseServices;

using HostMgd.ApplicationServices;

using App = HostMgd.ApplicationServices;
using Ed = HostMgd.EditorInput;
using Rtm = Teigha.Runtime;
#elif AC

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;

using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Gem = Autodesk.AutoCAD.Geometry;
using Ed = Autodesk.AutoCAD.EditorInput;
using Rtm = Autodesk.AutoCAD.Runtime;

#endif






namespace dRzTools.Servise
{
    /// <summary> Пути разделители и пр.</summary>
    public class SysInfo
    {
        #region Служебные
        /// <summary>Домен машины</summary>
        internal static string Userdomain = Environment.GetEnvironmentVariable("USERDOMAIN");

        /// <summary>Почта</summary>
        internal static string sMailTo = "mailto:"
            + SysConstant.sMail1
            + "? subject="
            + sTitleAttribute
            + " v"
            + sVersionFull
            + "&CC="
            + SysConstant.sMail2;


        #endregion
        #region ВЕРСИЯ ПРОГРАММЫ
        /// <summary>Версия программы</summary>
        internal static Version sysVersion => asm.GetName().Version;

        /// <summary>Версия мажор</summary>
        internal static int iMajor => sysVersion.Major;

        /// <summary>Версия минор</summary>
        internal static int iMinor => sysVersion.Minor;

        /// <summary>Версия билд</summary>
        public static int iBuild => sysVersion.Build;

        /// <summary>Версия ревизия</summary>
        internal static int iRevision => sysVersion.Revision;

        /// <summary>Бета или нет, чет нечет</summary>
        //static string _sBeta => iMinor == 0 || iMinor >3 ? "" : iMinor == 1 ? "<alfa>" : "<beta>";
        private static string _sBeta => iMinor > 0 & iMinor <= 5 ? "<beta>" : "";
        #endregion

        #region Assembly

        /// <summary>Сборка содержащая текущий исполняемый код</summary>
        public static Assembly asm => Assembly.GetExecutingAssembly();

        /// <summary> Титул программы</summary>
        public static string sTitleAttribute => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyTitleAttribute),
            false) as AssemblyTitleAttribute).Title;

        /// <summary>Описание программы </summary>
        public static string sDescription => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyDescriptionAttribute),
            false) as AssemblyDescriptionAttribute).Description;//!описание

        /// <summary>Конфигурация программы </summary>
        public static string sConfiguration => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyConfigurationAttribute),
            false) as AssemblyConfigurationAttribute).Configuration;

        /// <summary>Компания </summary>
        public static string sCompany => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyCompanyAttribute),
            false) as AssemblyCompanyAttribute).Company;

        /// <summary>Продукт </summary>
        public static string sProduct => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyProductAttribute),
            false) as AssemblyProductAttribute).Product;

        /// <summary>копирайт</summary>
        public static string sCopyright => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyCopyrightAttribute),
            false) as AssemblyCopyrightAttribute).Copyright;

        /// <summary>торговая марка</summary>
        public static string sTrademark => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyTrademarkAttribute),
            false) as AssemblyTrademarkAttribute).Trademark;

        /// <summary>ProductVersion - Версия программы
        /// <br>для идентификации в лицензии пограмма для нк или АК</br>
        /// </summary>
        public static string sInformationalVersionAttribut => (Attribute.GetCustomAttribute(
                   asm,
                   typeof(AssemblyInformationalVersionAttribute),
                   false) as AssemblyInformationalVersionAttribute).InformationalVersion;

        /// <summary>Полный путь к сборке </summary>
        public static string sAsmFulPath => asm.Location;

        /// <summary>Имя сборки без расширения</summary>
        public static string sAsmFileNameWithoutExtension => Path.GetFileNameWithoutExtension(sAsmFulPath);

        /// <summary>Имя сборки с расширением</summary>
        public static string sAsmFileName => Path.GetFileName(sAsmFulPath);

        /// <summary>версия программы </summary>
        public static string sVersion => iMajor.ToString()
                                         + "."
                                         + iMinor.ToString()
                                         + "."
                                         + iBuild.ToString()
                                         + _sBeta//_sysVersion.ToString() + _sBeta;
                                         ;
        /// <summary>Полная версия с окончанием </summary>
        public static string sVersionFull => iMajor.ToString()
                                             + "."
                                             + iMinor.ToString()
                                             + "."
                                             + iBuild.ToString()
                                             + "."
                                             + iRevision.ToString()
                                             + _sBeta;//_sysVersion.ToString() + _sBeta;

        /// <summary>Полная версия без окончания </summary>
        public static string sVersionShort => iMajor.ToString()
                                             + "."
                                             + iMinor.ToString()
                                             + "."
                                             + iBuild.ToString()
                                             + "."
                                             + iRevision.ToString();

        /// <summary>Дата сборки</summary>
        public static string sDateRelies
        {
            get
            {
                DateTime result = new DateTime(2000, 1, 1).AddDays(iBuild).AddSeconds(iRevision * 2);


#if DEBUG
                return result.ToString();
#else
                return result.ToLongDateString();
#endif
            }
        }


        /// <summary>Дата сборки</summary>
        public static string sDateRelis()
        {
            DateTime result = new DateTime(2000, 1, 1);
            result = result.AddDays(iBuild);
            result = result.AddSeconds(iRevision * 2);

#if DEBUG
            return result.ToString();
#else
            return result.ToLongDateString();
#endif



        }


        #endregion

        #region О CAD

        /// <summary>Название CAD программы </summary>
        internal static string sAppProductName => System.Windows.Forms.Application.ProductName;

        /// <summary>Мажор версия CAD программы </summary>
#if WPF// если отладка форм
        internal static string sAppMajor => ((System.Windows.Forms.Application.ProductVersion).Split('.'))[0];
#elif NC20//если кад

        internal static string sAppMajor => ((System.Windows.Forms.Application.ProductVersion).Split('.'))[0];

#else
        internal static string sAppMajor =>Application.Version.Major.ToString();

#endif

        #endregion

        #region  ПУТИ

        /// <summary>Путь к Program DATA</summary>
        private static string _sProgramData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        /// <summary>Общий путь к пользовательским настройкам 
        /// <br>c:\Users\User\AppData\Roaming\ </br></summary>
        private static string _sUserAppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>Путь к рабочему столу</summary>
        public static string sUserDesktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>Имя машины</summary>
        public static string sMachineName => Environment.MachineName;

        /// <summary>Путь к настройкам CAD версии </summary>
        private static string VersionSetingPath
        {
            get
            {
                string _VersionSetingPath;

                _VersionSetingPath = Path.Combine(_sUserAppData, sProduct, sAppProductName, sAppMajor);

                McUtilWorkFil.CreateDirMod(_VersionSetingPath);
                return _VersionSetingPath;
            }
        }

        /// <summary>Путь к глобальным настройкам программы 
        /// <br>для AutoCAD и nanoCAD всех версий одна настройка</br></summary>
        public static string GeneralSettingPath
        {
            get
            {
                string _GeneralSettingPath;
                _GeneralSettingPath = Path.Combine(_sUserAppData, sProduct);

                McUtilWorkFil.CreateDirMod(_GeneralSettingPath);

                return _GeneralSettingPath;
            }
        }

        /// <summary>Путь к файлу лицензии если есть</summary>
        internal static string LicPath
        {
            get
            {
                string _LicPath;
                _LicPath = Path.Combine(GeneralSettingPath, "lic");

                McUtilWorkFil.CreateDirMod(_LicPath);

                return _LicPath;
            }
        }

        /// <summary> Путь к настройкам метода печати  </summary>
        public static string setPlotSeting => Path.Combine(VersionSetingPath, "PlotSeting.ini");

        /// <summary> Путь к настройкам шаблонов  </summary>
        public static string setPlotTemplate => Path.Combine(VersionSetingPath, "PlotTemplate.ini");

        /// <summary> Путь к стилям печати, принтерам их бумаге (с размерами) </summary>
        public static string setPlotDevises => Path.Combine(VersionSetingPath, "PlotDevises.ini");

        /// <summary>Путь к главным настройкам</summary>
        public static string setGeneralSetting => Path.Combine(GeneralSettingPath, "GeneralSetting.ini");

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