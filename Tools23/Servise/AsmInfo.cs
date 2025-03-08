using System;
using System.IO;
using System.Reflection;

#if NET
[assembly: AssemblyInformationalVersion("AsmInfo for CAD")]
#endif

namespace drz.Infrastructure.CAD.Service
{
    /// <summary>
    ///
    /// </summary>
    public partial class AsmInfo
    {
        #region Служебные

        /// <summary>Домен машины</summary>
        public string userdomain => Environment.GetEnvironmentVariable("USERDOMAIN") ?? string.Empty;

        #endregion Служебные

        /// <summary>
        /// Initializes a new instance of the <see cref="AsmInfo"/> class.
        /// </summary>
        public AsmInfo()
        {
            asm = Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsmInfo"/> class.
        /// </summary>
        /// <param name="_asm">The asm.</param>
        public AsmInfo(Assembly _asm)
        {
            asm = _asm;
        }

        private Assembly asm { get; set; }

        #region ВЕРСИЯ ПРОГРАММЫ

        /// <summary>Версия программы</summary>
        public System.Version sysVersion => asm.GetName()?.Version ?? new Version();

        /// <summary>Версия мажор</summary>
        public int iMajor => sysVersion.Major;

        /// <summary>Версия минор</summary>
        internal int iMinor => sysVersion.Minor;

        /// <summary>Версия билд</summary>
        public int iBuild => sysVersion.Build;

        /// <summary>Версия ревизия</summary>
        internal int iRevision => sysVersion.Revision;

        /// <summary>Бета или нет, чет нечет</summary>
        //static string _sBeta => iMinor == 0 || iMinor >3 ? "" : iMinor == 1 ? "<alfa>" : "<beta>";
        private string _sBeta => iMinor < 2 ? "<alfa>" : iMinor < 4 ? "<beta>" : "";

        #endregion ВЕРСИЯ ПРОГРАММЫ

        #region Assembly

        /// <summary> Титул программы</summary>
        public string sTitleAttribute => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyTitleAttribute),
            false) as AssemblyTitleAttribute)?.Title ?? string.Empty;

        /// <summary>Описание программы </summary>
        public string sDescription => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyDescriptionAttribute),
            false) as AssemblyDescriptionAttribute)?.Description ?? string.Empty;//!описание

        /// <summary>Конфигурация программы </summary>
        public string sConfiguration => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyConfigurationAttribute),
            false) as AssemblyConfigurationAttribute)?.Configuration ?? string.Empty;

        /// <summary>Компания </summary>
        public string sCompany => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyCompanyAttribute),
            false) as AssemblyCompanyAttribute)?.Company ?? string.Empty;

        /// <summary>Продукт </summary>
        public string sProduct => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyProductAttribute),
            false) as AssemblyProductAttribute)?.Product ?? string.Empty;

        /// <summary>копирайт</summary>
        public string sCopyright => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyCopyrightAttribute),
            false) as AssemblyCopyrightAttribute)?.Copyright ?? string.Empty;

        /// <summary>торговая марка</summary>
        public string sTrademark => (Attribute.GetCustomAttribute(
            asm,
            typeof(AssemblyTrademarkAttribute),
            false) as AssemblyTrademarkAttribute)?.Trademark ?? string.Empty;

        /// <summary>ProductVersion - Версия программы
        /// <br>для идентификации в лицензии пограмма для нк или АК</br>
        /// </summary>
        public string sInformationalVersionAttribute => (Attribute.GetCustomAttribute(
                   asm,
                   typeof(AssemblyInformationalVersionAttribute),
                   false) as AssemblyInformationalVersionAttribute)?.InformationalVersion ?? string.Empty;

        /// <summary>Полный путь к сборке </summary>
        public string sAsmFullPath => asm.Location;

        /// <summary>Имя сборки без расширения</summary>
        public string sAsmFileNameWithoutExtension => Path.GetFileNameWithoutExtension(sAsmFullPath);

        /// <summary>Имя сборки с расширением</summary>
        public string sAsmFileName => Path.GetFileName(sAsmFullPath);

        /// <summary>версия программы </summary>
        public string sVersionBeta => iMajor.ToString()
                                         + "."
                                         + iMinor.ToString()
                                         + "."
                                         + iBuild.ToString()
                                         + _sBeta//_sysVersion.ToString() + _sBeta;
                                         ;

        /// <summary>Полная версия с окончанием </summary>
        public string sVersionFullBeta => iMajor.ToString()
                                             + "."
                                             + iMinor.ToString()
                                             + "."
                                             + iBuild.ToString()
                                             + "."
                                             + iRevision.ToString()
                                             + _sBeta;//_sysVersion.ToString() + _sBeta;

        /// <summary>
        /// Gets the s version full.
        /// </summary>
        /// <value>
        /// The s version full.
        /// </value>
        public string sVersionFull => sysVersion.ToString();

        /// <summary>Полная версия без окончания </summary>
        public string sVersionShort => iMajor.ToString()
                                             + "."
                                             + iMinor.ToString()
                                             + "."
                                             + iBuild.ToString()
                                             + "."
                                             + iRevision.ToString();

        /// <summary>Дата сборки</summary>
        public string sDateRelies
        {
            get
            {
                //DateTime result = new DateTime(2000, 1, 1);
                DateTime result = new DateTime(2000, 1, 1).AddDays(iBuild).AddSeconds(iRevision * 2);
                //result = result.AddDays(iBuild);
                //result = result.AddSeconds(iRevision * 2);

#if DEBUG
                return result.ToString();
#else
                return result.ToLongDateString();
#endif
            }
        }

        #endregion Assembly

        #region ПУТИ

        /// <summary>Путь к Program DATA</summary>
        public string sProgramData => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        /// <summary>Общий путь к пользовательским настройкам
        /// <br>c:\Users\User\AppData\Roaming\ </br></summary>
        public string sUserAppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>Путь к рабочему столу</summary>
        public string sUserDesktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>Имя машины</summary>
        public string sMachineName => Environment.MachineName;

        #endregion ПУТИ
    }
}