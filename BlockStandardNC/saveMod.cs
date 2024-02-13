using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using HostMgd.Windows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

using Teigha.DatabaseServices;
using Teigha.Runtime;

namespace drz.Tools
{
    /*public*/ class saveMod
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
            //?
            //пока сохраняем в текущей версии
            //  filedia 1 брать версию по индексу фильтра
            //  filedia 0 в ком строке словарик с предлагаемыми версиями
            //все это обернуть в класс просто вызов и тру фальш

            if (CadCommand.IsLisp)
            {
                ed.WriteMessage("Lisp started");
            }
            else
            {
                ed.WriteMessage("Lisp stopped");
            }

            PromptSaveFileOptions pfso = new PromptSaveFileOptions("Сохранить файл: ");

            pfso.Filter = "2018 (*.dwg)| *.dwg|"
                             + "2013 (*.dwg)|"
                             + "*.dwg"
                             ;
            pfso.FilterIndex = 2;
            PromptFileNameResult pfnr = ed.GetFileNameForSave(pfso);
            //DisplaySaveOptionsMenuItem  false   bool
            //PreferCommandLine   false   bool

            if (pfnr.Status != PromptStatus.OK) return;

            string fileName = pfnr.StringResult;

            //https://adn-cis.org/forum/index.php?topic=9134.msg37359#msg37359
            // DocumentSaveFormat defFormat = Application.DocumentManager.DefaultFormatForSave;

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
