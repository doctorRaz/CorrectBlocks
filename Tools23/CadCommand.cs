using System;

#if NC
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;


using Teigha.DatabaseServices;

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
using drz.Tools;

namespace DrzCadTools
{
    /// <summary> Вызов всех модулей 
    /// <br>добавил импорт фильтров слоев</br>
    /// <br>выбор блоков для вращения атрибутов </br> 
    /// 
    /// </summary>
    class CadCommand : Rtm.IExtensionApplication
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is lisp.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is lisp; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsLisp { get; set; }

#if NC
        #region Lsp
        // think убрать подписки и все что их касается, решение отличать команду от лисп выражения есть

        /// <summary>
        /// LWSs the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="LispWillStartEventArgs"/> instance containing the event data.</param>
        private /*static*/ void lws(object sender, LispWillStartEventArgs args)
        {
            IsLisp = true;
        }

        /// <summary>
        /// Lwes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private /*static*/ void lwe(object sender, EventArgs args)
        {
            IsLisp = false;
        }

        /// <summary>
        /// LispCancelled хз когда вызывается
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private /*static*/ void lwc(object sender, EventArgs args)
        {
            IsLisp = false;
        }

        #endregion
#endif
        #region INIT
        public void Initialize()
        {
            //think добавить проверку есть ли doc

            App.DocumentCollection dm = App.Application.DocumentManager;

#if NC
            dm.MdiActiveDocument.LispWillStart += new LispWillStartEventHandler(lws);
            dm.MdiActiveDocument.LispEnded += new EventHandler(lwe);
            dm.MdiActiveDocument.LispCancelled += new EventHandler(lwc);
#endif

            Ed.Editor ed = dm.MdiActiveDocument.Editor;
            //!+Вывожу список команд определенных в библиотеке
            ed.WriteMessage("\nStart list of commands: \n");
            string sCom =
                "drz_WipBot" + "\tМаскировки ВСЕХ блоков на задний план\n" +
                "drz_blc_WipBot" + "\tМаскировки ВЫБРАННЫХ блоков на задний план\n" +
                "drz_AtrRotateSel" + "\tКрутит в ВЫБРАННЫХ блоках ВСЕ атрибуты в ноль\n" +
                "drz_AtrRotateAll" + "\tКрутит в блоке ВСЕ атрибуты в ноль\n" +
                "drz_AtrRotateOne" + "\tКрутит выбранный атрибут блока в ноль\n" +
                "drz_blc_SetDlg" + "\tДиалоговая настройки нормализации блоков\n" +
                "drz_blc_EntityToZero" + "\tПримитивы на слой 0\n" +
                "drz_blc_ColorByLayer" + "\tЦвет примитивов по слою\n" +
                "drz_blc_AllPropByLayer" + "\tВсе свойства по слою\n" +
                "drz_blc_AllPropByBlock" + "\tВсе свойства по блоку\n" +
                "drz_AtrSynch" + "\tСинхронизация атрибутов от Андрея Бушмана\n" +
                "drz_AtrSynchHard" + "\tСинхронизация атрибутов от Gilles Chanteau (Грубое обновление)\n" +
                "drz_LayerImport" + "\tИмпорт фильтров слоев\n)" +
                "drz_rem_anntb" + "\tОтключение аннотативности ВСЕХ блоков\n" +
                "drz_rem_anntG" + "\tОтключение аннотативности ВСЕХ блоков\n" +
                "drz_rem_anntBent" + "\tОтключение аннотативности ВСЕХ блоков + снять аннотативность с сущностей\n"
                ;
            ed.WriteMessage(sCom);
            ed.WriteMessage("\nEnd list of commands\n");

#if DEBUG
            //для отладки список команд, чтоб лишнего не попало
            McUtilServise.FindCmdDuplicates();
#endif           
        }

        public void Terminate()
        {
            // throw new System.NotImplementedException();
        }

        #endregion

        #region Command

        #region Save

#if DEBUG
        [Rtm.CommandMethod("drz_SAVE_TEST")]//ok
        public static void SaveTest()
        {
            //Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ed.WriteMessage("CCC Ok");
            //---Y

            PromptSaveFileOptions pfso = new PromptSaveFileOptions("Сохранить файл: ");
            pfso.Filter =
                          "Drawing (*.dwg)|*.dwg|"
                        + "Design Web Format (*.dwf)|*.dwf|"
                        + "All files (*.*)|*.*";
            pfso.DialogCaption = "DialogCaption";
            pfso.DialogName = "DialogName";
            pfso.DeriveInitialFilenameFromDrawingName = true;
            pfso.InitialFileName = doc.Name;
            pfso.InitialDirectory = @"c:\temp\00";

            PromptFileNameResult pfnr = ed.GetFileNameForSave(pfso);

            if (pfnr.Status != PromptStatus.OK) return;

            string fileSaveName = pfnr.StringResult;

            db.SaveAs(fileSaveName, DwgVersion.Current);
        }
#endif

        #endregion

        #region Remove Annotate

        /// <summary>
        /// Отключение аннотативности ВСЕХ блоков
        /// </summary>
        [Rtm.CommandMethod("drz_rem_anntB")]//Ok
        //[Rtm.CommandMethod("drz_rem_anntB", Rtm.CommandFlags.Session | Rtm.CommandFlags.Modal)]
        public static void BlcRemovAnntCmd()
        {
            RemovAnnotate.Rem_annt(false);
        }

        /// <summary>
        /// Отключение аннотативности ВСЕХ блоков + снять аннотативность с сущностекй
        /// </summary>
        [Rtm.CommandMethod("drz_rem_anntBent")]//Ok
        //[Rtm.CommandMethod("drz_rem_anntBent", Rtm.CommandFlags.Session | Rtm.CommandFlags.Modal)]
        public static void BlcRemovAnntEntCmd()
        {
            RemovAnnotate.Rem_annt(true);
        }
        
        #endregion


        #region Маскировки

        /// <summary>
        /// Move to bottom in blocks wipe out
        /// </summary>
        [Rtm.CommandMethod("drz_WipBot")]//ok
        //[Rtm.CommandMethod("drz_WipBot", Rtm.CommandFlags.Modal)]
        public void Blc_WipeoutToBotton()
        {
            WipBot.WipeoutToBotton();
        }

        #endregion

        #region Атрибуты Крутим
        /// <summary>
        /// Крутит в выбранных блоках ВСЕ атрибуты в ноль
        /// </summary>
        [Rtm.CommandMethod("drz_AtrRotateSel")]//ok
        //[Rtm.CommandMethod("drz_AtrRotateSel", Rtm.CommandFlags.Modal)]
        public void AttS_RotateSelect()
        {
            //? сделать по аналогии с топить маскировки блоков
            AtrRotate.AttAllRotateSel();
        }

        /// <summary>
        /// Крутит в блоке ВСЕ атрибуты в ноль
        /// </summary>
        [Rtm.CommandMethod("drz_AtrRotateAll")]//Ok
        //[Rtm.CommandMethod("drz_AtrRotateAll", Rtm.CommandFlags.Modal)]
        public void AttS_Rotate()
        {
            AtrRotate.AttAllRotate();
        }

        ///  <summary>
        /// Крутит выбранный атрибут блока в ноль
        /// </summary>
        [Rtm.CommandMethod("drz_AtrRotateOne")]//Ok
        public void AttRotate()
        {
            AtrRotate.AttSingleRotate();
        }
        #endregion

        #region Атрибуты синх
        /// <summary>
        /// Синхронизация атрибутов от Андрея Бушмана
        /// </summary>
        [Rtm.CommandMethod("drz_AtrSynch")]//ok
        //[Rtm.CommandMethod("drz_AtrSynch", Rtm.CommandFlags.Modal)]
        public static void Att_MySynch()//кулик делал правки, проверить в бою, к AttSync добавил в класс остальные расширения
        {
            AttSynch.mySynch();
        }

        /// <summary>
        /// Синхронизация атрибутов от Gilles Chanteau (Грубое обновление)
        /// </summary>

        [Rtm.CommandMethod("drz_AtrSynchHard")]//ok
        //[Rtm.CommandMethod("drz_AtrSynchHard", Rtm.CommandFlags.Modal)]
        public void Att_MySynchHard()//кулик делал правки, проверить в бою, к AttSync добавил в класс остальные расширения
        {
            AttSynch.mySynchHard();
        }

        #endregion

        #region Починка блоков

        /// <summary>
        /// Топить маскировку выбранных блоков
        /// </summary>
        [Rtm.CommandMethod("drz_blc_WipBot")] //Ok
        public void Blc_WipBot()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetWipeoutBack;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Диалог настройки нормализации блоков
        /// <br> Все по слою (VL-CMDF "БЛКДЛГ" "Д" "Д" "С" "С" "С" "Д" "Д")</br>
        /// <br> Все по слою (VL-CMDF "drz-blc-SetDlg" "Д" "Д" "С" "С" "С" "Д" "Д")</br>
        /// </summary>
        [Rtm.CommandMethod("drz_blc_SetDlg")]//ok
        public void Blc_GetSetDlg()
        {
            NLB.GetBlc_SetDlg();
        }

        /// <summary>
        /// Примитивы на слой 0 
        /// </summary>
        [Rtm.CommandMethod("drz_blc_EntityToZero")]//ok
        public void Blc_Zero()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetLayer0;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Цвет  примитивов по слою
        /// </summary>
        [Rtm.CommandMethod("drz_blc_ColorByLayer")]//ok
        public void Blc_Color_Layer()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.ColorByLayer;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Цвет  примитивов по блоку
        /// </summary>
        [Rtm.CommandMethod("drz_blc_ColorByBlock")]//OK
        public void Blc_Color_Block()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.ColorByBlock;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Все свойства по слою
        /// </summary>
        [Rtm.CommandMethod("drz_blc_AllPropByLayer")]//ok
        public void Blc_All_Layer()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetBlockExplodeable
                                | NLB.BlockNormalizeSettingsEnum.EqualScaleOn
                                | NLB.BlockNormalizeSettingsEnum.SetByLayer
                                | NLB.BlockNormalizeSettingsEnum.ColorByLayer
                                | NLB.BlockNormalizeSettingsEnum.LineweightByLayer
                                | NLB.BlockNormalizeSettingsEnum.SetLayer0
                                | NLB.BlockNormalizeSettingsEnum.SetWipeoutBack
                                  ;
            NLB.GetBlc(fBlck);
        }

        /// <summary>
        /// Все свойства по блоку
        /// </summary>
        [Rtm.CommandMethod("drz_blc_AllPropByBlock")]//ok
        public void Blc_All_Block()
        {
            NLB.BlockNormalizeSettingsEnum fBlck = NLB.BlockNormalizeSettingsEnum.SetBlockExplodeable
                                | NLB.BlockNormalizeSettingsEnum.EqualScaleOn
                                | NLB.BlockNormalizeSettingsEnum.SetByBlock
                                | NLB.BlockNormalizeSettingsEnum.ColorByBlock
                                | NLB.BlockNormalizeSettingsEnum.LineweightByBlock
                                | NLB.BlockNormalizeSettingsEnum.SetLayer0
                                | NLB.BlockNormalizeSettingsEnum.SetWipeoutBack
                                 ;
            NLB.GetBlc(fBlck);
        }



        #endregion

        #endregion
    }
}
