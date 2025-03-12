//!отрефакторил
namespace dRzTools.Servise
{
    /// <summary> константы /const/</summary>
    public class DataSetWpfConst //: INotifyPropertyChanged
    {
        #region СЕКЦИЯ ЛИЦЕНЗИИ

#if NC || WPF
        //лицензия нанокад или WPF отладки
        public const string sSecLicense = "LicenseNC";
#elif AC
        public const string sSecLicense = "LicenseAC";
#endif

#region КЛЮЧИ

        /// <summary>Полный путь к файлу лицензии</summary>
        public const string sFileLicPath = "sFileLicPath";

        /// <summary>Флаг лицензии, есть нет на всякий</summary>
        public const string bLic = "bLic";

#endregion

#endregion

#region СПЕЦИАЛЬНЫЕ
#region РАЗДЕЛИТЕЛЬ склейки для ИНИ

        /// <summary>Разделитель склейки для ИНИ </summary>
        public const string setDelimiter = "‡¤¤¤‡";
#endregion
#endregion
        //***
#region СЕЦИЯ НАСТРОЕК ГЛАВНОГО ОКНА
        /// <summary>Секция позиции окна </summary>
        public const string sSecWpfPos = "WpfPos";
        //---
#region КЛЮЧИ
        /// <summary>Координата Х </summary>
        public const string sLeft = "dLeft";

        /// <summary>Координата Y </summary>
        public const string sTop = "dTop";
#endregion
#endregion

#region  СЕКЦИЯ О ПРОГАРММЕ

        /// <summary>Секция обновлений</summary>
        public const string sSecUpdate = "Update";

#region КЛЮЧИ

        /// <summary>Способ обновления
        /// <br>0- не проверять</br>
        /// <br>1-уведомлять</br>
        /// <br>2-автообновление</br>
        /// </summary>
        public const string sUpdateHow = "iUpdateHow";

        /// <summary>Дата последнего обновления</summary>
        public const string sUpdateDate = "UpdateDate";

#endregion

#region АДРЕСА

        /// <summary>Сервер обновлений</summary>
        public const string sUrlServer = "https://doctorraz.ucoz.ru/";

        /// <summary>Домашняя страница программы</summary>
        internal const string sHomePageURL = "https://doctorraz.blogspot.com/2022/10/PlotSPDS.NET.html";

        /// <summary>Что нового</summary>
        internal const string sWhatNew = "https://doctorraz.blogspot.com/p/0.html";

        /// <summary>Адрес инструкции по регистрации</summary>
        internal const string sHowToReg = "https://doctorraz.blogspot.com/p/blog-page_5.html";

        /// <summary>Сбор денег 300 р</summary>
        //internal const string sYoomoney300_URL = "https://doctorraz.blogspot.com/p/blog-page.html";     
        internal const string sYoomoney300_URL = "https://yoomoney.ru/bill/pay/15FOGMNU2G5.240925";//500 р

        /// <summary>Донат на юмани (кто сколько может)</summary>
        internal const string sYoomoneyDonate_URL = "https://yoomoney.ru/to/410013753090296";

        /// <summary>Адрес почты основной</summary>
        internal const string sMail1 = "doctorraz@yandex.ru";

        /// <summary>Адрес почты дополнительный</summary>
        internal const string sMail2 = "razygraev@gmail.com";

#endregion

#endregion
        //***
#region СЕЦИЯ НАСТРОЕК ПЕЧАТИ
        /// <summary>  Секция настроек печати  </summary>
        public const string sSecPlotSetup = "PlotSetup";
        //---
#region КЛЮЧИ

        /// <summary>Ключ  Флаги способ получения форматов для печати (Откуда печатать?)
        ///<br>flags:</br>
        ///<br>0-  fSelect выбрать на чертеже</br>
        ///<br>1-  fActiveLayout с активного пространства вкладки</br>
        ///<br>2-  fActiveFil активный документ</br>
        ///<br>3-  fOpenDocs открытые документы</br>
        ///<br>4-  fOpenFolder открыть из папки</br>
        ///<br>13- fPages только листы</br>
        ///<br>14- fModel только модель</br>
        /// </summary>
        public const string sKeySourceMethod = "fSourceMethod";

        /// <summary>Ключ Флаги метода сортировки 
        ///<br>flags:</br>
        ///<br>0-  fDesignShet Шифр лист </br>
        ///<br>1-  fUpLR верх потом направо</br>
        ///<br>16- fSyzeA4A0 По формату А4-А0</br>
        ///<br>17- fSyzeA0A4 По формату А0-А4</br>
        ///<br>31- fFilName По имени файла (быстрее вывод на бумагу)</br> 
        /// </summary>
        public const string sKeySortMethod = "fSortMethod";

        /// <summary>Ключ Состояние чек боксов фильтров слоев и переключателей видимости биты
        /// <br>bute:</br>
        /// <br>0- TBIfilter радио батон По списку</br>
        /// <br>1- TBImask радио батон По маске</br>
        /// <br>2- CHKlayerAllowLayer чек бокс Форматы со слоев</br>
        /// <br>3- CHKlayerAllLayer чек бокс Слои со всех форматов </br>
        /// </summary>
        public const string sKeyMaskFilterLayer = "fMaskFilterLayer";

        /// <summary>Ключ Состояние чекбоксов учитывать открытые файлы, подпапки, мультивыбор
        /// <br>bute:</br>
        /// <br>0- CHKwithSubfolders с подпапками</br>
        /// <br>1- CHKmultiSelect мультивыбор</br>
        /// <br>2- CHKandOpenFiles открытые файлы</br>
        /// </summary>
        public const string sKeySetFolderPlot = "fSetFolderPlot";

        /// <summary>Ключ Состояние чекбоксов и значение текстбокса: 
        /// Печать, копии прозрачность, открывать ли файл после печати
        /// <br>bute:</br>
        /// <br>0- CHKplotTransparent печать с прозрачностью</br>
        ///<br>1- CHKopenFil открыть файл после печати</br>
        ///<br>2- CHKSelectFolderOut учитывать путь напечатанных файлов</br>
        ///<br>3- TBXcopyCount_S количество печатных копий</br>
        /// </summary>
        public const string sKeyPlotPlot = "fPlotPlot";

        /// <summary>Ключ  Флаги метода компоновки  
        ///<br>flags:</br>
        ///<br>0- fFormatFil формат=файл</br>
        ///<br>1- fTopicFil раздел=файл</br>
        ///<br>2- fDocFil Документ=файл</br>
        ///<br>8- fAllDocFil все в файл</br> 
        /// </summary>
        public const string sKeyLayoutMethod = "fLayoutMethod";

        /// <summary>Ключ Флаги маски фильтров на что печатать 
        ///<br>flags:</br>
        ///<br>0- fPDF в PDF </br>
        ///<br>1- fEMF В EMF</br>
        ///<br>2- fDWF В DWF </br>
        ///<br>3- fDWFx В DWFx</br>
        ///<br>4 -fXPS XPS doc writer</br>
        ///<br>5- fRaster Растровый принтер</br>
        ///<br>6- fRasterToPDF Растр в PDF</br>
        ///<br>7- fPaper На железные принтеры</br>
        ///<br>8- fTemplateSet Принтеры из настроек</br>
        ///<br>9- fModelToPaper Модель разбить на листы</br> 
        /// </summary>
        public const string sKeyPlotDirectMethod = "fPlotDirectMethod";


        /// <summary>Ключ Состояние чекбоксов настроек ЛФМ: 
        /// Блокировать ВЭ,Удалять листы, ВЭ на непечатаемый слой
        /// <br>int:</br>
        ///<br>0- CHKisLockViewport блокировать ВЭ</br>
        ///<br>1- CHKisViewportonUnplotLayer ВЭ на непечатаемый слой</br>
        ///<br>2- CHKisDeleteLayouts удалять листы</br>
        ///<br>3 RBTisDeleteLayoutsAll удалять все листы</br>
        ///<br>4- RBTisDeleteLayoutsPlotSPDS удалять только листы PlotSPDS</br>
        ///<br>5- RBTisDeleteLayoutsUser удалять только пользовательские листы</br>
        ///<br>6- CHKisReturnModel После создания листов возвращаться в модель</br>
        ///<br>7- CHKisSowStile Показывать стили печати</br>
        ///<br>8- CHKisPageSet Layout or PageSet</br>
        ///<br>9- CHKisLayoutShortName Короткое имя листа</br>     
        /// </summary>
        public const string sKeyLFM = "fLFM";

        /// <summary>
        /// Автосохранение настроек шаблонов
        /// <br>true - автосохранение</br>
        /// <br>false - надо жать кнопку сохранить шаблон</br>
        /// </summary>
        public const string sKeyAutoSaveTemplate = "bAutoSaveTemplate";

        /// <summary>Ключ Состояние чекбоксов как печать на бумагу и в PDF
        /// <br>0-Индекс бумага шаблона</br>
        /// <br>8- индекс бумага стиля</br>
        /// <br>16- индекс PDF стиля</br>
        /// <br>24- индекс PDF принтера</br>
        /// <br>32- индекс растрового стиля</br>
        /// <br>40- индекс растрового принтера</br>
        /// <br>48- индекс разрешение DPI</br>
        /// <br>59- включать закладки в PDF</br>
        /// <br>60-чек бокс выбрать стиль или из настроек бумага</br>
        /// <br>61-чек бокс PDF внедрять SHX</br>
        /// </summary>
        public const string sKeyPlotMethod = "fPlotMethod ";

        /// <summary>Ключ Строка маски фильтров слоев </summary>
        public const string sKeyLayersMask = "sLayersMask";

        /// <summary>Ключ Коллекция выбранных слоев (через разделитель) </summary>
        public const string sKeyLayersSelected = "sLayersSelected";

        /// <summary>Ключ Папка чертежей откуда печатать</summary>
        public const string sKeyFolderPlot = "sFolderPlot";

        /// <summary>Ключ Папка напечатанных файлов</summary>
        public const string sKeyFolderOut = "sFolderOut";

#endregion
#endregion
        //***
#region СЕЦИЯ ПЛОТЕРОВ 

        /// <summary> Секция цветозависимых стилей печати  </summary>
        public const string sSecPlotStiles_Color = "PlotStiles_Color";

        /// <summary> Секция именованных стилей печати  </summary>
        public const string sSecPlotStiles_Named = "PlotStiles_Named";

        /// <summary> Секция ВСЕХ стилей печати  </summary>
        public const string sSecPlotStiles_All = "PlotStiles_All";

        /// <summary> Секция всех принтеров  </summary>
        public const string sSecPlotAll = "PlotAll";

        /// <summary> Секция железных принтеров  </summary>
        public const string sSecPlotHard = "PlotHard";

        /// <summary> Секция PDF принтеров  </summary>
        public const string sSecPlotPDF = "PlotPDF";

        /// <summary> Секция растровых принтеров  </summary>
        public const string sSecPlotRaster = "PlotRaster";

#region КЛЮЧИ ПЛОТЕРОВ

#endregion

#endregion
        //***
#region РАЗМЕРЫ ШАБЛОНОВ /пока три/
        /// <summary>Ключ малого шаблона</summary>
        public const string sSmalKeyTemplate = "Sml";

        /// <summary>Ключ среднего шаблона</summary>
        public const string sMidKeyTemplate = "Mid";

        /// <summary>Ключ большого шаблона</summary>
        public const string sLagKeyTemplate = "Lag";

#endregion
    }
}
