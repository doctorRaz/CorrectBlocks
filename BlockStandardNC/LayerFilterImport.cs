using System.IO;
using System.Windows.Forms;

using DialogResult = System.Windows.Forms.DialogResult;

#if NC

using App = HostMgd.ApplicationServices;

using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;
using Teigha.LayerManager;

using Db = Teigha.DatabaseServices;

#elif AC
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Gem = Autodesk.AutoCAD.Geometry;
using Ed = Autodesk.AutoCAD.EditorInput;
using Rtm = Autodesk.AutoCAD.Runtime;
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
                openFileDialog.Filter = "Все поддерживаемые форматы (*.dwg;*.dws;*.dwt;*.dxf)|*.dwg;*.dws;*.dwt;*.dxf";
                openFileDialog.Title = "Выберите чертеж для импорта фильтров слоев";
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
        public static void ImportLFilterFromFile(string sourcefile)
        {
            // Find the file containing layer filter to clone
            if (!File.Exists(sourcefile))
            {
                MessageBox.Show("Could not find file !");
                return;
            }
       
            DocumentCollection dm = App.Application.DocumentManager;

            Document doc = dm.MdiActiveDocument;

            Database db = doc.Database;

            Editor ed = doc.Editor;

            try
            {
                using (Db.Database db0 = new Db.Database(false, false))
                {
#if NC
                    db0.ReadDwgFile(sourcefile, Db.FileOpenMode.OpenForReadAndAllShare, false, "", false);
#else
                    db0.ReadDwgFile(sourcefile, Db.FileOpenMode.OpenForReadAndAllShare, false, "");
#endif
                    Db.HostApplicationServices.WorkingDatabase = db0;

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

                Db.HostApplicationServices.WorkingDatabase = db;
                ed.WriteMessage("\nFilter import completed successfully");
            }

            catch (System.Exception ex)
            {
                App.Application.ShowAlertDialog(string.Format("\n{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
