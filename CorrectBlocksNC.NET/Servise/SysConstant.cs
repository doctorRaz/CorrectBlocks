


namespace dRzTools.Servise
{
    /// <summary> константы /const/</summary>
    public class SysConstant //: INotifyPropertyChanged
    {
        #region СЕКЦИЯ ЛИЦЕНЗИИ

#if NC || WPF
        //лицензия нанокад или WPF отладки
        public const string sSecLicense = "LicenseNC";
#elif AC
        public const string sSecLicense = "LicenseAC";
#endif

        #endregion

        #region  СЕКЦИЯ О ПРОГАРММЕ
  

        #region АДРЕСА

        /// <summary>Сервер обновлений</summary>
        public const string sUrlServer = "https://doctorraz.ucoz.ru/";

        /// <summary>Домашняя страница программы</summary>
        internal const string sHomePageURL = "https://doctorraz.blogspot.com/2022/10/PlotSPDS.NET.html";

        /// <summary>Что нового</summary>
        internal const string sWhatNew = "https://doctorraz.blogspot.com/p/0.html";

        /// <summary>Адрес инструкции по регистрации</summary>
        internal const string sHowToReg = "https://doctorraz.blogspot.com/p/blog-page_5.html";

        /// <summary>Сбор денег 500 р</summary>
        //internal const string sYoomoney300_URL = "https://doctorraz.blogspot.com/p/blog-page.html";     
        internal const string sYoomoney_URL = "https://yoomoney.ru/bill/pay/15FOGMNU2G5.240925";//500 р

        /// <summary>Донат на юмани (кто сколько может)</summary>
        internal const string sYoomoneyDonate_URL = "https://yoomoney.ru/to/410013753090296";

        /// <summary>Адрес почты основной</summary>
        internal const string sMail1 = "doctorraz@yandex.ru";

        /// <summary>Адрес почты дополнительный</summary>
        internal const string sMail2 = "razygraev@gmail.com";

        #endregion

        #endregion

    }
}
