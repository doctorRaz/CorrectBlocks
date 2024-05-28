
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swf = System.Windows.Forms;
using System.IO;
using Microsoft.Win32;






//using System.Windows.Forms;
#if NC
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using HostMgd.Windows;


using Teigha.DatabaseServices;
using Rtm=Teigha.Runtime;
#else

using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Rtm = Autodesk.AutoCAD.Runtime;


#endif

namespace drz.Tools
{
    //? нифига не доделано, слепил в кучу, шоб хоть както работал из ком строки аналог автокадовского СОХРАНИТЬ
    //сохраняет только в последней версии и только в dwg
    class saveMod
    {
        /// <summary>
        /// Saves the mod.
        /// </summary>
        //[CommandMethod("drz_save")]
        public static void SaveMod()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;            

            //ful name
            string sFilName = doc.Name;

            sFilName = Path.GetFullPath(sFilName);//затычка если файл открыт по относительному пути

            string sSaveDir;

            McUtilServise.RegistryMod rm = new McUtilServise.RegistryMod();

            bool isFilExist = File.Exists(sFilName);
            if (isFilExist)
            {
                sSaveDir = Path.GetDirectoryName(sFilName);
            }
            else
            {
                sSaveDir = rm.objSaveInitDir.ToString();
            }


            PromptSaveFileOptions pfso = new PromptSaveFileOptions("Сохранить файл: ");
            //https://www.keanw.com/2009/08/allowing-a-user-to-select-from-multiple-file-formats-inside-autocad-using-net.html
            pfso.Filter =
                  "Drawing (*.dwg)|*.dwg|"
                + "Design Web Format (*.dwf)|*.dwf|"
                + "All files (*.*)|*.*";
            pfso.DialogCaption = "DialogCaption";
            pfso.DialogName = "DialogName";
            pfso.DeriveInitialFilenameFromDrawingName = true;
            pfso.InitialFileName = doc.Name;
            pfso.InitialDirectory = sSaveDir;


            /*
             //pfso.PreferCommandLine = true;  
             //pfso.Filter = "2018 (*.dwg)|*.dwg|"
             //            + "2013 (*.dwg)|*.dwg|"
             //            + "2010 (*.dwg)|*.dwg|"
             //            + "2007 (*.dwg)|*.dwg|"
             //            + "2004 (*.dwg)|*.dwg|"
             //            + "2000 (*.dwg)|*.dwg"
             //;
             //pfso.FilterIndex = 0;
            */
            object cmdactive = Application.GetSystemVariable("CMDACTIVE");

            if ((int)cmdactive == 0)
            {//window
                pfso.PreferCommandLine = false;
            }
            else
            {//cmd
                pfso.PreferCommandLine = true;
                pfso.Message = "Сохранить файл: <" + Path.Combine(pfso.InitialDirectory, pfso.InitialFileName) + ">";
            }

            PromptFileNameResult pfnr = ed.GetFileNameForSave(pfso);

            if (pfnr.Status != PromptStatus.OK) return;

            string fileSaveName = pfnr.StringResult;
            //? затычка шоб это работало 
            fileSaveName =Path.ChangeExtension(fileSaveName, "dwg");

            //? поведение АК
            /*
                файл вообще без каталога посылает нах (можем писать в последнюю сохраненную)
                не сохраненный файл без расширения сохраняет в dwg  версия последняя заданная
                сохраненный файл с расширением и без сохраняет в его версии
                сохраненный 
                сохраняет в той же версии и типе файла если нет расширения
                тип файла определяет по расширению
                
                для dxf запрашивает точность всегда

                если некорректные символы алерт
                несуществующий путь открывает файл диалог
                
                файл существует просит разрешения на перезапись (да/нет)
                       
            ------------------------
           для ком строки просто отработать сохранение в dwg в current версии
            проверять пути и запрещенные символы и ошибку записи файла если косяк АЛЕРТ
            перезаписывать сущ файл молча

            */
            //think проверить имя на допустимые символы и наличие директорий



            //? вот не надо этот путь сохранять save path from registry
            //rm.objSaveInitDir = Path.GetDirectoryName(fileSaveName);


            //https://adn-cis.org/forum/index.php?topic=9134.msg37359#msg37359

            //think обернуть  try catch обработать
            try
            {
                db.SaveAs(fileSaveName, DwgVersion.Current);
            }
            catch (Rtm.Exception ex)
            {
                ed.WriteMessage(ex.Message);
                ed.WriteMessage(ex.StackTrace);
            }


            return;



            object objFILEDIA = Application.GetSystemVariable("FILEDIA");

            ed.WriteMessage("CMDACTIVE\t" + cmdactive.ToString());
            /*
            Активные команды отсутствуют	0
            Активна обычная команда	1
            Активна прозрачная команда	2
            Активен сценарий	4
            Активно диалоговое окно	8
            Активен динамический обмен данными (DDE)	16
            Активен AutoLISP (отображается только для команды, определенной в ObjectARX)	32
            Активна команда ObjectARX	64
            */

            //Application.SetSystemVariable("FILEDIA", FILEDIA );

            bool IsFILEDIA = (int)objFILEDIA == 1;

            if (CadCommand.IsLisp || (int)cmdactive != 0)
            {// ком строка



                ed.WriteMessage("Lisp started");
            }
            else
            {//диалог save
                // HKEY_CURRENT_USER\SOFTWARE\Nanosoft\nanoCAD x64\23.1\Profiles\SPDS\IO\SaveProjects SaveInitDir
                // jgty
                // HKEY_CURRENT_USER\SOFTWARE\Nanosoft\nanoCAD x64\23.1\Profiles\SPDS\IO\AllOpenFileFormats OpenInitDir

                swf.SaveFileDialog dlg = new swf.SaveFileDialog()
                {
                    Filter =
                       "AutOCAD 2018 (*.dwg)|*.dwg|"
                     + "AutOCAD 2013 (*.dwg)|*.dwg|"
                     + "AutOCAD 2010 (*.dwg)|*.dwg|"
                     + "AutOCAD 2007 (*.dwg)|*.dwg|"
                     + "AutOCAD 2004 (*.dwg)|*.dwg|"
                     + "AutOCAD 2000 (*.dwg)|*.dwg"
                     ,
                    AddExtension = true,
                    Title = "Сохранить файл: ",
                    FileName = doc.Name,
                    //RestoreDirectory = true
                    //InitialDirectory=
                };

                if (dlg.ShowDialog() != swf.DialogResult.OK) return;


                ed.WriteMessage("Lisp stopped");
            }

            //PromptSaveFileOptions pfso = new PromptSaveFileOptions("Сохранить файл: ");

            //https://www.keanw.com/2009/08/allowing-a-user-to-select-from-multiple-file-formats-inside-autocad-using-net.html
            pfso.Filter =
                  "Drawing (*.dwg)|*.dwg|"
                + "Design Web Format (*.dwf)|*.dwf|"
                + "All files (*.*)|*.*";
            //pfso.DialogCaption = "Caption";
            //pfso.DialogName = "Name";
            //pfso.DeriveInitialFilenameFromDrawingName = true;
            //think добавить текущую директорию

            pfso.InitialFileName = doc.Name;
            //pfso.InitialDirectory = "";
            //pfso.PreferCommandLine = true;  
            //pfso.Filter = "2018 (*.dwg)|*.dwg|"
            //            + "2013 (*.dwg)|*.dwg|"
            //            + "2010 (*.dwg)|*.dwg|"
            //            + "2007 (*.dwg)|*.dwg|"
            //            + "2004 (*.dwg)|*.dwg|"
            //            + "2000 (*.dwg)|*.dwg"
            //;
            //pfso.FilterIndex = 0;
            //PromptFileNameResult pfnr = ed.GetFileNameForSave(pfso);
            //DisplaySaveOptionsMenuItem  false   bool
            //PreferCommandLine   false   bool

            if (pfnr.Status != PromptStatus.OK) return;

            string fileName = pfnr.StringResult;

            //https://adn-cis.org/forum/index.php?topic=9134.msg37359#msg37359
            // DocumentSaveFormat defFormat = Application.DocumentManager.DefaultFormatForSave;

            //think обернуть  try catch
            db.SaveAs(fileName, DwgVersion.Current);
            /*


            */
            //***

            //System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog()
            //{
            //    Filter = "dwg2000|*.dwg|dwg2007|*.dwg",
            //    AddExtension = true,
            //};
            //dlg.ShowDialog();

            //System.Windows.Forms.MessageBox.Show(dlg.FileName/*SafeFileName*/);
            //System.Windows.Forms.MessageBox.Show(dlg.FilterIndex.ToString());

            //SaveFileDialog sfo = new SaveFileDialog("title", "def name", "*.dwg", "dialog name",SaveFileDialog.SaveFileDialogFlags.AllowAnyExtension);
            //var ress = sfo.ShowDialog();
            //var ress2 = sfo.ShowModal();

            //OpenFileDialog ofd = new OpenFileDialog(OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder);

            //PlatformDb.DatabaseServices.TransactionManager tm = db.TransactionManager;


        }
    }
}

//***R





