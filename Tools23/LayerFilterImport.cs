using System.IO;
using System.Windows.Forms;

using DialogResult = System.Windows.Forms.DialogResult;



#if NC

using HostMgd.ApplicationServices;
using Application = HostMgd.ApplicationServices.Application;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;
using Teigha.LayerManager;

using  Teigha.DatabaseServices;

#elif AC
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
#endif

namespace drz.Tools
{
    public class LFI /*: IExtensionApplication*/
    {
        /// <summary>
        /// https://www.caduser.ru/forum/post278685.html#p278685
        /// Импорт фильтров слоев из файла в активный файл
        /// </summary>
        public static void LFilterImp()
        {
            string sFilName = string.Empty;

            // set full path of container file that contains desired layer filter
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All supported formats (*.dwg;*.dws;*.dwt;*.dxf)|*.dwg;*.dws;*.dwt;*.dxf";
                openFileDialog.Title = "Select drawing file to import layer filters";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                sFilName = openFileDialog.FileName;
            }

            ImportLFilterFromFile(sFilName);
        }

        /// <summary>
        /// Импорт слоев в активный файл
        /// </summary>
        /// <param name="sourcefile">Путь к файлу с фильтрами</param>
        private static void ImportLFilterFromFile(string sourcefile)
        {
            // Find the file containing layer filter to clone
            if (!File.Exists(sourcefile))
            {
                MessageBox.Show("Could not find file !");
                return;
            }
       
            DocumentCollection dm = Application.DocumentManager;

            Document doc = dm.MdiActiveDocument;

            Database db = doc.Database;

            Editor ed = doc.Editor;

            try
            {
                using (Database db0 = new Database(false, false))
                {
#if NC
                    db0.ReadDwgFile(sourcefile, FileOpenMode.OpenForReadAndAllShare, false, "", false);
#else
                    db0.ReadDwgFile(sourcefile, FileOpenMode.OpenForReadAndAllShare, false, "");
#endif
                    HostApplicationServices.WorkingDatabase = db0;

                    using (Transaction sourcetr = db0.TransactionManager.StartTransaction())
                    {
                        BlockTable sourcebt = null;

                        sourcebt = sourcetr.GetObject(db0.BlockTableId, OpenMode.ForRead) as BlockTable;

                        if (db0.LayerFilters.Root == null)
                        {
                            MessageBox.Show("The Layer Filters aren't found !");
                            return;
                        }

                        LayerFilterTree lt = db0.LayerFilters;

                        using (DocumentLock doclock = doc.LockDocument())
                        {
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                LayerFilterTree ltc = db.LayerFilters;

                                dm.MdiActiveDocument = doc;

                                db.LayerFilters = lt;

                                sourcetr.Commit();

                                tr.Commit();
                            }
                        }
                    }
                }

                HostApplicationServices.WorkingDatabase = db;
                ed.WriteMessage("\nFilter import completed successfully");
            }

            catch (System.Exception ex)
            {
                Application.ShowAlertDialog(string.Format("\n{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
