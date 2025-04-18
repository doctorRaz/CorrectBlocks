using System.ComponentModel;
using System.Reflection;

using drzTools.Block;

using drzTools.Servise;

using drzTools.SaveMods;



#if NC
using App = HostMgd.ApplicationServices;
using Ed = HostMgd.EditorInput;
using Rtm = Teigha.Runtime;

#elif AC

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Ed = Autodesk.AutoCAD.EditorInput;
using Gem = Autodesk.AutoCAD.Geometry;
using Rtm = Autodesk.AutoCAD.Runtime;

#endif

#if NET
[assembly: AssemblyInformationalVersion("drzTools for CAD")]
#endif

namespace drzTools.CadCommands
{
    /// <summary> Вызов всех модулей 
    /// <br>добавил импорт фильтров слоев</br>
    /// <br>выбор блоков для вращения атрибутов </br> 
    /// </summary>
    class CadCommand : Rtm.IExtensionApplication
    {
        #region INIT
        public void Initialize()
        {
            ListCmdInfo.ListCMD();//выводим список команд с описаниями


        }

        public void Terminate()
        {
            // throw new System.NotImplementedException();
        }

        #endregion

        #region Command

        #region INFO

        [Rtm.CommandMethod("drz_ToolsInfo")]
        [Description("Информация о командах сборки")]
        public static void ListCMD()
        {
            ListCmdInfo.ListCMD();//выводим список команд с описаниями
        }

        #endregion

        #region Save

        /// <summary>
        /// Аналог команды SAVE AutoCAD.
        /// </summary>
        [Rtm.CommandMethod("drz_save")]
        [Description("Аналог команды SAVE AutoCAD, сохраняет копию чертежа с другим именем, но рабочим остается прежний чертеж")]
        public static void SaveCmd()
        {
            SaveMod.Save_mod();
        }

        /// <summary>
        /// Программный Save as by Keanw
        /// </summary>
        [Rtm.CommandMethod("drz_QSAVEAS")]
        [Description("Программное СОХРАНИТЬ КАК от Keanw")]
        public static void QSaveAsCmd()
        {
            QSaveAsKeanw.QuickSaveAs();
        }


        [Rtm.CommandMethod("drz_Qsave")]
        [Description("Программное СОХРАНИТЬ от Кулик")]
        public static void QSaveCmdKpblcCmd()
        {
            QsaveKpblc.QuickSave();
        }

        [Rtm.CommandMethod("drz_SAVE_TEST")]
        [Description("Программно СОХРАНИТЬ ТЕСТ в разработке")]
        public static void Save_testCmd()
        {
            SaveTest.Save_Test();
        }

        #endregion

        #region Remove Annotate

        /// <summary>
        /// Отключение аннотативности ВСЕХ блоков
        /// </summary>
        [Rtm.CommandMethod("drz_rem_annt")]
        [Description("Отключение аннотативности ВСЕХ блоков")]
        public static void RemovAnntBlcCmd()
        {
            RemovAnnotate.Rem_annt(false);
        }

        /// <summary>
        /// Отключение аннотативности ВСЕХ блоков + снять аннотативность с примитивов
        /// </summary>
        [Rtm.CommandMethod("drz_rem_anntE")]
        [Description("Отключение аннотативности ВСЕХ блоков и отключить аннотативность с примитивов")]
        public static void RemovAnntAllCmd()
        {
            RemovAnnotate.Rem_annt(true);
        }

        #endregion


        #region Маскировки

        /// <summary>
        /// Маскировки ВСЕХ блоков на задний план
        /// </summary>
        [Rtm.CommandMethod("drz_WipBot")]
        [Description("Переместить маскировки ВСЕХ блоков на задний план")]
        public static void Blc_WipeoutToBotton()
        {
            Down.Wipeout();
        }

        /// <summary>
        /// Штриховки ВСЕХ блоков на задний план
        /// </summary>
        [Rtm.CommandMethod("drz_HatchBot")]
        [Description("Переместить штриховки ВСЕХ блоков на задний план")]
        public static void Blc_HatchToBotton()
        {
            Down.Hatch();
        }

        #endregion

        #region Атрибуты Крутим
        /// <summary>
        /// Крутит в выбранных блоках ВСЕ атрибуты в ноль
        /// </summary>
        [Rtm.CommandMethod("drz_AtrRotateSel")]
        [Description("Повернуть в ВЫБРАННЫХ блоках ВСЕ атрибуты в ноль")]
        public static void AttS_RotateSelect()
        {
            //? сделать по аналогии с топить маскировки блоков
            AtrRotate.AttAllRotateSel();
        }

        /// <summary>
        /// Крутит в блоке ВСЕ атрибуты в ноль
        /// </summary>
        [Rtm.CommandMethod("drz_AtrRotateAll")]
        [Description("Повернуть в блоке ВСЕ атрибуты в ноль")]
        public static void AttS_Rotate()
        {
            AtrRotate.AttAllRotate();
        }

        ///  <summary>
        /// Крутит выбранный атрибут блока в ноль
        /// </summary>
        [Rtm.CommandMethod("drz_AtrRotateOne")]
        [Description("Повернуть выбранный атрибут блока в ноль")]
        public static void AttRotate()
        {
            AtrRotate.AttSingleRotate();
        }
        #endregion

        #region Атрибуты синх
        /// <summary>
        /// Синхронизация атрибутов от Андрея Бушмана
        /// </summary>
        [Rtm.CommandMethod("drz_AtrSynch")]
        [Description("Синхронизация атрибутов от Андрея Бушмана, атрибуты остаются на своих местах и не меняют размер")]
        public static void Att_MySynch()//x кулик делал правки, проверить в бою, к AttSync добавил в класс остальные расширения
        {
            AttSynch.mySynch();
        }

        /// <summary>
        /// Синхронизация атрибутов от Gilles Chanteau (Грубое обновление)
        /// </summary>

        [Rtm.CommandMethod("drz_AtrSynchHard")]
        [Description("Синхронизация атрибутов от Gilles Chanteau (Грубое обновление), размер и положение блоков, как в референтом")]
        public static void AtrSynchHard()//кулик делал правки, проверить в бою, к AttSync добавил в класс остальные расширения
        {
            AttSynch.mySynchHard();
        }

        #endregion

        #region Починка блоков

        /// <summary>
        /// Топить маскировку выбранных блоков
        /// </summary>
        [Rtm.CommandMethod("drz_blc_WipBot")]
        [Description("Маскировки ВЫБРАННЫХ блоков на задний план")]
        public static void Blc_WipBot()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetWipeoutBack;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Топить штриховку выбранных блоков
        /// </summary>
        [Rtm.CommandMethod("drz_blc_HatchBot")]
        [Description("Штриховки ВЫБРАННЫХ блоков на задний план")]
        public static void Blc_HatchBot()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetHatchtBack;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Диалог настройки нормализации блоков
        /// <br> Все по слою (VL-CMDF "БЛКДЛГ" "Д" "Д" "С" "С" "С" "Д" "Д")</br>
        /// <br> Все по слою (VL-CMDF "drz-blc-SetDlg" "Д" "Д" "С" "С" "С" "Д" "Д")</br>
        /// </summary>
        [Rtm.CommandMethod("drz_blc_SetDlg")]
        [Description("Диалог настройки нормализации блоков, командная строка")]
        public static void Blc_GetSetDlg()
        {
            NLB.GetBlc_SetDlg();
        }

        /// <summary>
        /// Примитивы на слой 0 
        /// </summary>
        [Rtm.CommandMethod("drz_blc_EntityToZero")]
        [Description("Примитивы блока на слой 0")]
        public static void Blc_Zero()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetLayer0;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Цвет  примитивов по слою
        /// </summary>
        [Rtm.CommandMethod("drz_blc_ColorByLayer")]
        [Description("Цвет примитивов блока по слою")]
        public static void Blc_Color_Layer()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.ColorByLayer;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Цвет  примитивов по блоку
        /// </summary>
        [Rtm.CommandMethod("drz_blc_ColorByBlock")]
        [Description("Цвет примитивов блока по блоку")]
        public static void Blc_Color_Block()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.ColorByBlock;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Все свойства по слою
        /// </summary>
        [Rtm.CommandMethod("drz_blc_AllPropByLayer")]
        [Description("Все свойства примитивов блока по слою")]
        public static void Blc_All_Layer()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetBlockExplodeable
                                | NLB.BlockNormalizeSettingsEnum.EqualScaleOn
                                | NLB.BlockNormalizeSettingsEnum.LineTypeByLayer
                                | NLB.BlockNormalizeSettingsEnum.ColorByLayer
                                | NLB.BlockNormalizeSettingsEnum.LineWeightByLayer
                                | NLB.BlockNormalizeSettingsEnum.SetLayer0
                                | NLB.BlockNormalizeSettingsEnum.SetWipeoutBack
                                  ;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Все свойства по блоку
        /// </summary>
        [Rtm.CommandMethod("drz_blc_AllPropByBlock")]
        [Description("Все свойства примитивов блока по блоку")]
        public static void Blc_All_Block()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetBlockExplodeable
                                | NLB.BlockNormalizeSettingsEnum.EqualScaleOn
                                | NLB.BlockNormalizeSettingsEnum.LineTypeByBlock
                                | NLB.BlockNormalizeSettingsEnum.ColorByBlock
                                | NLB.BlockNormalizeSettingsEnum.LineWeightByBlock
                                | NLB.BlockNormalizeSettingsEnum.SetLayer0
                                | NLB.BlockNormalizeSettingsEnum.SetWipeoutBack
                                 ;
            NLB.GetBlc(fBlck);
        }



        #endregion

        #endregion
    }
}
